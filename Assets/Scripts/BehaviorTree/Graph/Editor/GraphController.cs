using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ExtensionMethods;
using Type = System.Type;
using System.Diagnostics;

namespace Benco.Graph
{
    public class GraphController
    {
        class EventState
        {
            public ModifierKeys modifiers { get; set; }
            public MouseButtons mouseButtons { get; set; }
            public EventType eventType { get; set; }
            public string eventCommand { get; set; }
        }

        public static NodeGraph graph { get { return NodeEditorWindow.instance.currentGraph; } }

        public Event lastMouseEvent { get; private set; }
        public Event lastKeyEvent { get; private set; }

        EventState currentEventState = new EventState();
        UIEvent currentEvent = null;

        Dictionary<Type, List<UIEvent>> registeredEvents = new Dictionary<Type, List<UIEvent>>();

        public GraphController()
        {
            GraphUIEvents graphEvents = new GraphUIEvents(NodeAttributeTags.GetNodeMenu<BehaviorTree.NodeComposite>());
            registeredEvents.Add(typeof(NodeGraph), graphEvents.graphEvents);
            registeredEvents.Add(typeof(NodeBase), graphEvents.nodeEvents);
            Undo.undoRedoPerformed += delegate { NodeEditorWindow.instance.Repaint(); };
        }

        [Conditional("Debug")]
        private void PrintAllNodes()
        {
            graph.edges.PrintAll(x => x == null ? "null" :
                               x.source == null ? "x.null" :
                          x.source.node == null ? "x.source.null" : x.source.node.name);
        }

        public void UpdateGraphGUI(Event e, Rect viewRect)
        {
            ProcessEvents(e, viewRect);
            PrintAllNodes();
            foreach (NodeEdge edge in graph.edges)
            {
                DrawEdge(edge);
            }
            foreach (NodeBase node in graph.nodes)
            {
                DrawNode(node);
            }
        }

        void ProcessEvents(Event e, Rect viewRect)
        {
            if (e.type == EventType.Repaint)
            {
                if (currentEvent != null)
                {
                    currentEvent.onRepaint(e);
                }
                return;
            }
            else if (e.type == EventType.Layout || e.type == EventType.Used)
            {
                return;
            }

            Type selectedType = Selection.activeObject != null ? Selection.activeObject.GetType() : typeof(void);

            foreach (Type type in registeredEvents.Keys)
            {
                if (type.IsAssignableFrom(selectedType))
                {
                    selectedType = type;
                    break;
                }
            }

            if (!registeredEvents.ContainsKey(selectedType))
            {
                Selection.activeObject = graph;
                selectedType = Selection.activeObject.GetType();
                //return;
            }

            foreach (Type type in registeredEvents.Keys)
            {
                if (type.IsAssignableFrom(selectedType))
                {
                    selectedType = type;
                    break;
                }
            }

            // Sum up the modification buttons
            currentEventState.modifiers = 0;
            currentEventState.modifiers |= (e.control ? ModifierKeys.Control : 0);
            currentEventState.modifiers |= (e.alt ? ModifierKeys.Alt : 0);
            currentEventState.modifiers |= (e.shift ? ModifierKeys.Shift : 0);
            // Add ModifierKeys.None if no modifier key was pressed.
            if (currentEventState.modifiers == 0) { currentEventState.modifiers = ModifierKeys.None; }
            currentEventState.eventCommand = e.commandName;
            currentEventState.eventType = e.type;
            // If a mouse button was pressed, update currentEventState.mouseButtons.
            if (e.type == EventType.MouseDown)
            {
                currentEventState.mouseButtons |= (MouseButtons)(1 << e.button);
            }

            bool newEvent = false;
            if (currentEvent == null)
            {
                for (int i = 0; i < registeredEvents[selectedType].Count; i++)
                {
                    UIEvent potentialEvent = registeredEvents[selectedType][i];
                    if (HasCorrectModifiers(currentEventState, potentialEvent) &&
                        HasCorrectMouseButtons(currentEventState, potentialEvent) &&
                        HasCorrectEvent(currentEventState, potentialEvent))
                    {
                        currentEvent = potentialEvent;
                        newEvent = true;
                        break;
                    }
                }
            }
            // Note: not same if statment because currentEvent can be set in the previous if statement.
            if (currentEvent != null)
            {
                if (newEvent)
                {
                    if (currentEvent.eventType == EventType.MouseDrag)
                    {
                        currentEvent.onEventBegin(lastMouseEvent);
                        currentEvent.onEventUpdate(e);
                    }
                    else if (currentEvent.eventType == EventType.MouseDown || 
                             currentEvent.eventType == EventType.MouseUp ||
                             currentEvent.eventType == EventType.ValidateCommand ||
                             currentEvent.eventType == EventType.ExecuteCommand)
                    {
                        currentEvent.onEventBegin(e);
                        currentEvent.onEventUpdate(e);
                        currentEvent.onEventExit(e);
                        currentEvent = null;
                    }
                    else
                    {
                        currentEvent.onEventBegin(e);
                        currentEvent.onEventUpdate(e);
                    }
                }
                else
                {
                    if (e.type == EventType.MouseUp && currentEvent.eventType == EventType.MouseDrag)
                    {
                        currentEvent.onEventUpdate(e);
                        currentEvent.onEventExit(e);
                        currentEvent = null;
                        lastMouseEvent = null;
                    }
                    else if ((currentEventState.mouseButtons == MouseButtons.Both &&
                              currentEvent.cancelOnBothMouseButtonsPressed) || e.keyCode == KeyCode.Escape)
                    {
                        currentEvent.onEventCancel(e);
                        currentEvent = null;
                        lastMouseEvent = null;
                        lastKeyEvent = null;
                    }
                    else
                    {
                        currentEvent.onEventUpdate(e);
                    }
                }
            }
            // If a button was released, remove it for the next event frame.
            if (e.type == EventType.MouseUp)
            {
                currentEventState.mouseButtons &= (MouseButtons)~(1 << e.button);
            }
            if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp)
            {
                lastMouseEvent = new Event(e);
            }
            else if (e.type == EventType.KeyDown || e.type == EventType.KeyUp)
            {
                lastKeyEvent = new Event(e);
            }
        }
        
        private bool HasCorrectModifiers(EventState eventState, UIEvent uiEvent)
        {
            if (uiEvent.mustHaveAllModifiers)
            {
                return (uiEvent.modifiers == eventState.modifiers);
            }
            else
            {
                return (uiEvent.modifiers & eventState.modifiers) > 0;
            }
        }

        private bool HasCorrectMouseButtons(EventState eventState, UIEvent uiEvent)
        {
            if(uiEvent.mustHaveAllMouseButtons)
            {
                return uiEvent.mouseButtons == eventState.mouseButtons;
            }
            else
            {
                return (uiEvent.mouseButtons & eventState.mouseButtons) > 0;
            }
        }

        private bool HasCorrectEvent(EventState eventState, UIEvent uiEvent)
        {
            if (uiEvent.eventType == EventType.ValidateCommand ||
                uiEvent.eventType == EventType.ExecuteCommand)
            {
                if (eventState.eventType == EventType.ValidateCommand ||
                eventState.eventType == EventType.ExecuteCommand)
                {
                    string[] commands = uiEvent.eventCommand.Split('|');
                    for (int i = 0; i < commands.Length; i++)
                    {
                        if (commands[i] == eventState.eventCommand)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else if (uiEvent.eventType == EventType.MouseDrag)
            {
                return lastMouseEvent != null &&
                       lastMouseEvent.type == EventType.MouseDown &&
                       eventState.eventType == EventType.MouseDrag;
            }
            else
            {
                return eventState.eventType == uiEvent.eventType;
            }
        }

        private void NodeDelete(Event e, NodeBase mainNode)
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (e.control)
                {
                    if (mainNode.isSelected)
                    {
                        Selection.objects = (from obj in Selection.objects
                                             where obj != mainNode
                                             select obj).ToArray();
                    }
                    else
                    {
                        Selection.objects = Selection.objects.Concat(new Object[] { mainNode }).ToArray();
                    }
                }
                else
                {
                    if (!mainNode.isSelected)
                    {
                        Selection.activeObject = mainNode;
                    }
                }
            }
        }

        void DrawNode(NodeBase node)
        {
            GUIStyle nodeStyle;

            if (node.isSelected || node.isHighlighted)
            {
                if ((graph.root == node))
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 3 on");
                }
                else
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 0 on");
                }
            }
            else
            {
                if (graph.root == node)
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 3");
                }
                else
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 0");
                }
            }

            GUI.Box(node.rect, node.title, nodeStyle);
            
            DrawConnections(node);
            EditorUtility.SetDirty(node);
        }

        void DrawEdge(NodeEdge edge)
        {
            Texture2D edgeTexture;
            if (edge.isSelected || edge.isHighlighted)
            {
                edgeTexture = GUI.skin.GetStyle("button").focused.background;
            }
            else
            {
                edgeTexture = GUI.skin.GetStyle("selectionRect").active.background;
            }
            // counterClockwiseOffset makes the directed edges offset. This splits the directed
            // edges so they do not align:
            //   ____  /__________  ____
            //  /  |R\ \           /  |R\
            // |  <>  |           |  <>  | (Radius R, <> == center of node)
            //  \____/___________\ \____/
            //                   /
            if (edge.edgeType == EdgeType.Directed)
            {
                Vector2 directionVector = edge.destination.node.rect.center - edge.source.node.rect.center;
                // The math below gets the Right vector from the directionVector above.
                Vector2 counterClockwiseOffset = new Vector2(directionVector.y, -directionVector.x);
                counterClockwiseOffset.Normalize();
                counterClockwiseOffset *= 7.0f;
                Handles.DrawAAPolyLine(edgeTexture, 3,
                    edge.source.node.rect.center + counterClockwiseOffset,
                    edge.destination.node.rect.center + counterClockwiseOffset);
            }
            else
            {
                Handles.DrawAAPolyLine(edgeTexture, 3,
                    edge.source.node.rect.center,
                    edge.destination.node.rect.center);
            }
        }

        void DrawConnections(NodeBase node)
        {
            //foreach (NodeBase connectedNode in node.output.nodes)
            //{
            //    Vector3 start = node.rect.center;
            //    Vector3 end = connectedNode.rect.center;
            //    Vector2 startTangent = new Vector2();
            //    Vector2 endTangent = new Vector2();
            //    Color color = Color.gray;

            //    if (node.isSelected || (node.input.connectedNode != null && node.input.connectedNode.isSelected))
            //    {
            //        color = new Color(1, 0.5f, 0);
            //    }

            //    float offset = Mathf.Abs(start.x - end.x) / 1.75f;
            //    if (end.x < start.x)
            //    {
            //        startTangent = new Vector2(start.x - offset, start.y);
            //        endTangent = new Vector2(end.x + offset, end.y);
            //    }
            //    else
            //    {
            //        startTangent = new Vector2(start.x + offset, start.y);
            //        endTangent = new Vector2(end.x - offset, end.y);
            //    }

            //    Handles.BeginGUI();
            //    {
            //        Handles.color = Color.white;
            //        Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 2);
            //    }
            //    Handles.EndGUI();
            //}
        }

        void DrawConnectionToMouse(Vector2 position, NodeBase selectedNode)
        {
            Vector3 start = selectedNode.rect.center;
            Vector3 end = position;
            Vector2 startTangent = new Vector2();
            Vector2 endTangent = new Vector2();
            Color color = Color.gray;
            color = new Color(1, 0.5f, 0);
            float offset = Mathf.Abs(start.x - end.x) / 1.75f;
            if (position.x < start.x)
            {
                startTangent = new Vector2(start.x - offset, start.y);
                endTangent = new Vector2(end.x + offset, end.y);
            }
            else
            {
                startTangent = new Vector2(start.x + offset, start.y);
                endTangent = new Vector2(end.x - offset, end.y);
            }

            Handles.BeginGUI();
            {
                Handles.color = Color.white;
                Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 2);
            }
            Handles.EndGUI();
        }

        public void Reset()
        {
            lastMouseEvent = null;
            lastKeyEvent = null;
            currentEventState = new EventState();
            currentEvent = null;
        }
    }
}

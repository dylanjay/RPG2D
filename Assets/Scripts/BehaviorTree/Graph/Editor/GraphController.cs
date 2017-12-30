using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ExtensionMethods;
using Conditional = System.Diagnostics.ConditionalAttribute;
using Type = System.Type;
using Benco.BehaviorTree;
using Benco.Utilities;

namespace Benco.Graph
{
    public class GraphController
    {
        UIEventEngine uiEventEngine = new UIEventEngine();
        class EventState
        {
            public ModifierKeys modifiers { get; set; }
            public MouseButtons mouseButtons { get; set; }
            public EventType eventType { get; set; }
            public string eventCommand { get; set; }
        }

        public static NodeGraph graph { get { return NodeEditorWindow.instance.currentGraph; } }
        
        Dictionary<Type, List<UIEvent>> registeredEvents = new Dictionary<Type, List<UIEvent>>();

        public Vector2 offset = new Vector2(0, 0);
        public Vector2 scale = new Vector2(1, 1);

        public GraphController()
        {
            GraphUIEvents graphEvents = new GraphUIEvents(this,
                                                          NodeAttributeTags.GetNodeMenu<NodeComposite>());
            registeredEvents.Add(typeof(NodeGraph), graphEvents.graphEvents);
            registeredEvents.Add(typeof(NodeBase), graphEvents.nodeEvents);
            registeredEvents.Add(typeof(NodeEdge), graphEvents.edgeEvents);
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
            GUI.Box(viewRect, "", GUI.skin.GetStyle("flow background"));
            if (graph != null)
            {
                PrintAllNodes();
                Matrix4x4 guiMatrix = Matrix4x4.identity;
                guiMatrix.m00 = scale.x;
                guiMatrix.m11 = scale.y;
                guiMatrix.m03 = (int)offset.x;
                guiMatrix.m13 = (int)offset.y;
                GUI.matrix = guiMatrix;
                Rect guiRect = GUIExtensions.BeginTrueClip(guiMatrix, viewRect);
                {
                    DrawGrid(viewRect, guiRect);
                    foreach (NodeEdge edge in graph.edges)
                    {
                        DrawEdge(edge);
                    }
                    foreach (NodeBase node in graph.nodes)
                    {
                        DrawNode(node);
                    }
                    OnGUI(e);
                }
                GUIExtensions.EndTrueClip();
                GUI.matrix = Matrix4x4.identity;
            }
        }

        private void DrawGrid(Rect viewRect, Rect guiRect)
        {
            float zoomPower = (Mathf.Log10(Mathf.Max(viewRect.width, viewRect.height)) - .5f
                               - Mathf.Log10(scale.x)) / Mathf.Log10(17f);

            // Split grid draws into 2 zoom groups: rounded down power of 2 and rounded up power of two
            int lowPower = (int)zoomPower;
            float lowZoomPower = (1 - (zoomPower - (int)zoomPower));
            lowZoomPower *= lowZoomPower;
            int lowLineSpacing = Mathf.RoundToInt(Mathf.Pow(10f, lowPower));
            int highPower = lowPower + 1;
            float highZoomPower = (1 - lowZoomPower);
            int highLineSpacing = Mathf.RoundToInt(Mathf.Pow(10f, highPower));
            Color prevHandleColor = Handles.color;
            DrawVerticalLines(guiRect, lowZoomPower, lowLineSpacing);
            DrawVerticalLines(guiRect, highZoomPower, highLineSpacing);
            DrawHorizontalLines(guiRect, lowZoomPower, lowLineSpacing);
            DrawHorizontalLines(guiRect, highZoomPower, highLineSpacing);

            Handles.color = prevHandleColor;
        }

        private void DrawVerticalLines(Rect guiRect, float zoomPower, float lineSpacing)
        {
            float gridOffsetX = guiRect.x - Mathf.Repeat(guiRect.x, lineSpacing);

            Handles.color = new Color(0.1f, 0.1f, 0.1f, zoomPower);
            for (int i = 0; i < guiRect.width / lineSpacing + 1; i++)
            {
                Handles.DrawAAPolyLine(1.0f / scale.x, 2, new Vector2(lineSpacing * i + gridOffsetX,
                                                                      (int)guiRect.y),
                                                          new Vector2(lineSpacing * i + gridOffsetX,
                                                                      (int)(guiRect.y + guiRect.height)));
            }
        }

        private void DrawHorizontalLines(Rect guiRect, float zoomPower, float lineSpacing)
        {
            float gridOffsetY = guiRect.y - Mathf.Repeat(guiRect.y, lineSpacing);

            Handles.color = new Color(0.1f, 0.1f, 0.1f, zoomPower);
            for (int i = 0; i < guiRect.height / lineSpacing + 1; i++)
            {
                Handles.DrawAAPolyLine(1.0f / scale.x, 2, new Vector2((int)guiRect.x,
                                                                      lineSpacing * i + gridOffsetY),
                                                          new Vector2((int)(guiRect.x + guiRect.width),
                                                                      lineSpacing * i + gridOffsetY));
            }
        }

        public void OnGUI(Event e)
        {
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
            }

            foreach (Type type in registeredEvents.Keys)
            {
                if (type.IsAssignableFrom(selectedType))
                {
                    selectedType = type;
                    break;
                }
            }
            uiEventEngine.eventList = registeredEvents[selectedType];
            uiEventEngine.OnGUI(e);
        }

        //TODO(mderu): Move this out of GraphController.
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
            EditorUtility.SetDirty(node);
        }

        //TODO(mderu): Move this out of GraphController.
        void DrawEdge(NodeEdge edge)
        {
            Color oldColor = Handles.color;
            if (edge.isSelected || edge.isHighlighted)
            {
                Handles.color = new Color(0.42f, 0.7f, 1.0f);
            }
            Vector2 startPosition, endPosition;
            edge.GetPoints(out startPosition, out endPosition);
            if (edge.edgeType == EdgeType.Directed)
            {
                float arrowWidth = 6.0f;

                Vector2 line = endPosition - startPosition;
                // Below are the 3 points of the arrow of the directed edge.
                Vector2 forwardPoint = line.normalized * 7;
                Vector2 leftPoint = forwardPoint.RotatedBy(120);
                Vector2 rightPoint = forwardPoint.RotatedBy(240);

                // The arrows Unity's Animation Window uses are centered around the cartesian center, 
                // rather than the arrow's center of mass. To mimic Unity, we'll calculate the cartesian
                // center of the arrow in offsetMidpoint.
                // If we decide later we don't care for this, we can use:
                //  Vector2 midpoint = startPosition + line / 2.0f;
                Vector2 offsetMidpoint = line.WithMagnitude(
                    ((Mathf.Cos(60.0f * Mathf.Deg2Rad) - 1.0f) * arrowWidth + line.magnitude) / 2.0f) + startPosition;
                Handles.DrawAAConvexPolygon(
                    offsetMidpoint + forwardPoint,
                    offsetMidpoint + leftPoint,
                    offsetMidpoint + rightPoint,
                    offsetMidpoint + forwardPoint);
            }
            Handles.DrawAAPolyLine(3, 2, startPosition, endPosition);
            Handles.color = oldColor;
        }

        public void Reset()
        {
            uiEventEngine.Reset();
        }
    }
}

using System.Collections.Generic;
using Benco.Graph;
using UnityEngine;
using UnityEditor;
using ExtensionMethods;
using System.Linq;

namespace Benco.BehaviorTree
{
    public class GraphUIEvents
    {
        /// <summary>
        /// The first position of the marquee selection box.
        /// </summary>
        private Vector2 firstSelectionPosition = Vector2.zero;
        /// <summary>
        /// The second position of the marquee selection box.
        /// </summary>
        private Vector2 secondSelectionPosition = Vector2.zero;

        private Vector2 dragStartLocation = Vector2.zero;
        private NodeBase startTransitionNode = null;

        public Rect selectionRect
        {
            get
            {
                float startX = Mathf.Min(firstSelectionPosition.x, secondSelectionPosition.x);
                float width = firstSelectionPosition.x + secondSelectionPosition.x - startX - startX;

                float startY = Mathf.Min(firstSelectionPosition.y, secondSelectionPosition.y);
                float height = firstSelectionPosition.y + secondSelectionPosition.y - startY - startY;
                return new Rect(startX, startY, width, height);
            }
        }

        public List<UIEvent> graphEvents;
        public List<UIEvent> nodeEvents;

        private static readonly Texture2D lineTexture = GUI.skin.GetStyle("selectionRect").active.background;

        public GraphUIEvents()
        {
            graphEvents = new List<UIEvent>()
            {
                new UIEvent("Create Selection Box")
                {
                    mouseButtons = MouseButtons.Left,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => StartSelection(e),
                    onEventUpdate = (Event e) => UpdateSelection(e),
                    onEventExit = (Event e) => ExitSelection(e, canceled: false),
                    onEventCancel = (Event e) => ExitSelection(e, canceled: true),
                    onRepaint = (Event e) => { GUI.Box(selectionRect, "", GUI.skin.GetStyle("selectionRect")); }
                },

                new UIEvent("Right Click Menu")
                {
                    mouseButtons = MouseButtons.Right,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseUp,
                    onEventExit = (Event e) => { NodeAttributeTags.GetNodeMenu<NodeComposite>().ShowAsContext(); },
                },


                new UIEvent("Drag All Nodes")
                {
                    mouseButtons = MouseButtons.Middle,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Drag(e.delta, GraphController.graph.nodes),
                    onEventCancel = (Event e) => Drag(e.mousePosition - dragStartLocation, 
                                                      GraphController.graph.nodes),
                },

                new UIEvent("Attempt Select Node")
                {
                    mouseButtons = MouseButtons.Left | MouseButtons.Right,
                    mustHaveAllMouseButtons = false,
                    modifiers = ModifierKeys.None | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt,
                    mustHaveAllModifiers = false,
                    eventType = EventType.MouseDown,
                    onEventBegin = (Event e) => AttemptNodeSelect(e)
                },
            };

            nodeEvents = new List<UIEvent>()
            {
                new UIEvent("Attempt Select Node")
                {
                    mouseButtons = MouseButtons.Left | MouseButtons.Right,
                    mustHaveAllMouseButtons = false,
                    modifiers = ModifierKeys.None | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt,
                    mustHaveAllModifiers = false,
                    eventType = EventType.MouseDown,
                    onEventBegin = (Event e) => AttemptNodeSelect(e)
                },

                new UIEvent("Reposition/Drag")
                {
                    mouseButtons = MouseButtons.Left,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Drag(e.delta, from obj in Selection.objects
                                                               where typeof(NodeBase).IsAssignableFrom(obj.GetType())
                                                               select (NodeBase)obj),
                    onEventCancel = (Event e) => Drag(e.mousePosition - dragStartLocation, 
                                                      from obj in Selection.objects
                                                      where typeof(NodeBase).IsAssignableFrom(obj.GetType())
                                                      select (NodeBase)obj),
                },

                new UIEvent("Drag All Nodes")
                {
                    mouseButtons = MouseButtons.Middle,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Drag(e.delta, GraphController.graph.nodes),
                    onEventCancel = (Event e) => Drag(e.mousePosition - dragStartLocation,
                                                      GraphController.graph.nodes),
                },

                new UIEvent("Delete Nodes")
                {
                    mouseButtons = MouseButtons.None,
                    modifiers = ModifierKeys.None,
                    mustHaveAllModifiers = true,
                    eventType = EventType.ValidateCommand,
                    eventCommand = "SoftDelete|Delete",
                    onEventBegin = (Event e) => DeletePressed(e)
                },

                new UIEvent("Right Click Menu")
                {
                    mouseButtons = MouseButtons.Right,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseUp,
                    onEventExit = (Event e) => ShowNodeContextMenu(e),
                },

                new UIEvent("Create Transition")
                {
                    mouseButtons = MouseButtons.Left,
                    modifiers = ModifierKeys.Alt,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => StartTransition(e), // ,
                    onEventExit = (Event e) => EndTransition(e),
                    onEventUpdate = (Event e) => { NodeEditorWindow.instance.Repaint(); },
                    onRepaint = (Event e) => RepaintTransition(e),
                }
            };
        }

        private void StartTransition(Event e)
        {
            // Make sure there is a node that is selected first.
            NodeBase selectedNode = Selection.activeObject as NodeBase;
            if (selectedNode == null)
            {
                return;
                //AttemptNodeSelect(e);
                //selectedNode = Selection.activeObject as NodeBase;
            }
            startTransitionNode = selectedNode;
            dragStartLocation = e.mousePosition;
        }

        private void RepaintTransition(Event e)
        {
            Handles.DrawAAPolyLine(lineTexture, 3, dragStartLocation, e.mousePosition);
        }

        private void EndTransition(Event e)
        {
            NodeBase nodeUnderMouse = GetNodeAt(e.mousePosition);
            if (nodeUnderMouse != null)
            {
                if (NodeEdge.CanCreateEdge(startTransitionNode, nodeUnderMouse))
                {
                    NodeEdge.CreateEdge(startTransitionNode, nodeUnderMouse);
                }
            }
            startTransitionNode = null;
        }

        private void CancelTransition(Event e)
        {
            startTransitionNode = null;
        }

        private void ShowNodeContextMenu(Event e)
        {
            if (Selection.activeObject)
            {
                if (typeof(NodeBase).IsAssignableFrom(Selection.activeObject.GetType()))
                {
                    ((NodeBase)Selection.activeObject).ShowContextMenu(e);
                }
            }
        }

        private NodeBase GetNodeAt(Vector2 position)
        {
            foreach (NodeBase node in GraphController.graph.nodes)
            {
                if (node.rect.Contains(position))
                {
                    return node;
                }
            }
            return null;
        }

        private void AttemptNodeSelect(Event e)
        {
            foreach (NodeBase node in GraphController.graph.nodes)
            {
                if (node.rect.Contains(e.mousePosition))
                {
                    if (e.control)
                    {
                        Selection.objects = Selection.objects.CopyPushFront(node);
                    }
                    else
                    {
                        if (!Selection.objects.Contains(node))
                        {
                            Selection.activeObject = node;
                        }
                    }
                    NodeEditorWindow.instance.Repaint();
                    return;
                }
            }

            //Nothing was selected and no modifiers were pressed, select the graph.
            if (!e.alt && !e.control && !e.shift)
            {
                Selection.activeObject = GraphController.graph;
            }
            NodeEditorWindow.instance.Repaint();
        }

        private void DragAll(Vector2 delta)
        {
            foreach (NodeBase node in GraphController.graph.nodes)
            {
                node.rect.x += delta.x;
                node.rect.y += delta.y;
            }
            NodeEditorWindow.instance.Repaint();
        }

        private void Drag(Vector2 delta, IEnumerable<NodeBase> nodes)
        {
            foreach (NodeBase node in nodes)
            {
                node.rect.x += delta.x;
                node.rect.y += delta.y;
            }
            NodeEditorWindow.instance.Repaint();
        }

        private void DeletePressed(Event e)
        {
            Object[] selectedObjects = Selection.objects;
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                NodeBase nodeBase = selectedObjects[i] as NodeBase;
                if (nodeBase != null)
                {
                    GraphController.graph.DeleteNode(nodeBase);
                }
            }
            NodeEditorWindow.instance.Repaint();
        }

        private void StartSelection(Event e)
        {
            firstSelectionPosition = e.mousePosition;
        }

        private void UpdateSelection(Event e)
        {
            secondSelectionPosition = e.mousePosition;
            foreach (NodeBase node in GraphController.graph.nodes)
            {
                node.isHighlighted = selectionRect.Overlaps(node.rect);
            }
            NodeEditorWindow.instance.Repaint();
        }

        private void ExitSelection(Event e, bool canceled)
        {
            bool addedNode = false;
            List<Object> selection = new List<Object>(Selection.objects);
            foreach (NodeBase node in GraphController.graph.nodes)
            {
                if (selectionRect.Overlaps(node.rect))
                {
                    selection.Add(node);
                    addedNode = true;
                }
                node.isHighlighted = false;
            }

            if (!canceled)
            {
                if (addedNode)
                {
                    selection.Remove(GraphController.graph);
                }
                Selection.objects = selection.ToArray();
            }
        }
    }
}

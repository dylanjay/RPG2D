using System.Collections.Generic;
using Benco.Graph;
using UnityEngine;
using UnityEditor;
using ExtensionMethods;
using System.Linq;
using Benco.Utilities;

namespace Benco.Graph
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

        /// <summary>
        /// Used for dragging objects from a starting location. For click-and-drag/selection,
        /// <see cref="firstSelectionPosition"/>
        /// <seealso cref="secondSelectionPosition"/>
        /// </summary>
        private Vector2 dragStartLocation = Vector2.zero;

        /// <summary>
        /// The node an edge is being created from.
        /// </summary>
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
        public List<UIEvent> edgeEvents;

        private static readonly Texture2D lineTexture = GUI.skin.GetStyle("selectionRect").active.background;

        public GraphUIEvents(GenericMenu contextMenu)
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
                    onEventExit = (Event e) => { contextMenu.ShowAsContext(); },
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

                new UIEvent("Drag All Nodes")
                {
                    mouseButtons = MouseButtons.Left,
                    modifiers = ModifierKeys.Alt,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Drag(e.delta, GraphController.graph.nodes),
                    onEventCancel = (Event e) => Drag(e.mousePosition - dragStartLocation,
                                                      GraphController.graph.nodes),
                },

                new UIEvent("Attempt Select Nodes or Edges")
                {
                    // UnityShenanigans:
                    // Right clicking a node will also set it as selected. This is default in Unity.
                    // Note that right clicking an edge will not select it. This is handled in 
                    // AttemptNodeObjectSelection()
                    mouseButtons = MouseButtons.Left | MouseButtons.Right,
                    mustHaveAllMouseButtons = false,
                    modifiers = ModifierKeys.None | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt,
                    mustHaveAllModifiers = false,
                    eventType = EventType.MouseDown,
                    onEventBegin = (Event e) => AttemptNodeObjectSelection(e)
                },
            };

            nodeEvents = new List<UIEvent>()
            {
                new UIEvent("Attempt Select Nodes or Edges")
                {
                    mouseButtons = MouseButtons.Left | MouseButtons.Right,
                    mustHaveAllMouseButtons = false,
                    modifiers = ModifierKeys.None | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt,
                    mustHaveAllModifiers = false,
                    eventType = EventType.MouseDown,
                    onEventBegin = (Event e) => AttemptNodeObjectSelection(e)
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
                    checkedOnEventBegin = (Event e) => StartTransition(e),
                    onEventExit = (Event e) => EndTransition(e),
                    onEventUpdate = (Event e) => { NodeEditorWindow.instance.Repaint(); },
                    onRepaint = (Event e) => RepaintTransition(e),
                },
            };

            edgeEvents = new List<UIEvent>()
            {
                new UIEvent("Attempt Select Node or Edges")
                {
                    mouseButtons = MouseButtons.Left | MouseButtons.Right,
                    mustHaveAllMouseButtons = false,
                    modifiers = ModifierKeys.None | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt,
                    mustHaveAllModifiers = false,
                    eventType = EventType.MouseDown,
                    onEventBegin = (Event e) => AttemptNodeObjectSelection(e)
                },

                // UnityShenanigans:
                // When clicking on an edge, you can continue to drag to create a selection box
                // to select nodes. This also adds the selected edge to the selection box as well.
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
            };
        }
        
        private bool StartTransition(Event e)
        {
            // Make sure there is a node that is selected first.
            NodeBase selectedNode = Selection.activeObject as NodeBase;
            if (selectedNode == null || !selectedNode.rect.Contains(e.mousePosition))
            {
                return false;
            }
            startTransitionNode = selectedNode;
            dragStartLocation = e.mousePosition;
            return true;
        }

        private void RepaintTransition(Event e)
        {
            Handles.DrawAAPolyLine(lineTexture, 3, dragStartLocation, e.mousePosition);
            //TODO(mderu): Change this to if (edges are directed for this type of graph)
            if (true)
            {
                Vector2 line = e.mousePosition - dragStartLocation;
                Vector2 midpoint = dragStartLocation + line / 2.0f;
                // Below are the 3 points of the arrow of the directed edge.
                Vector2 forwardPoint = line.normalized * 7;
                Vector2 leftPoint = forwardPoint.RotatedBy(120);
                Vector2 rightPoint = forwardPoint.RotatedBy(240);
                Handles.DrawAAConvexPolygon(
                    midpoint + forwardPoint,
                    midpoint + leftPoint,
                    midpoint + rightPoint,
                    midpoint + forwardPoint);
            }
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

        private void AttemptNodeObjectSelection(Event e)
        {
            // UnityShenanigans:
            // If you Ctrl/Shift + Click a node/edge twice in a row without moving the mouse,
            // It doesn't update the selection on the second click. The third click will
            // toggle the selection as expected.

            foreach (NodeBase node in GraphController.graph.nodes)
            {
                if (node.rect.Contains(e.mousePosition))
                {
                    if ((e.control || e.shift) && !Selection.objects.Contains(GraphController.graph))
                    {
                        int index = ArrayUtility.FindIndex(Selection.objects, (nodeBase) => nodeBase == node);
                        if (index >= 0)
                        {
                            Selection.objects = Selection.objects.WithIndexRemoved(index);
                        }
                        else
                        {
                            Selection.objects = Selection.objects.CopyPushFront(node);
                        }
                    }
                    else
                    {
                        // UnityShenanigans:
                        // If a selected node is clicked on without holding control or shift,
                        // it will not deselect the other selected items.
                        if (!Selection.objects.Contains(node))
                        {
                            Selection.activeObject = node;
                        }
                    }
                    NodeEditorWindow.instance.Repaint();
                    return;
                }
            }

            // Unity by default will not select edges if a right/context-click has been recieved.
            // Don't ask me why, I don't make up the rules.
            if (e.button != 1 && !e.alt)
            {
                foreach (NodeEdge edge in GraphController.graph.edges)
                {
                    Vector2 startPoint, endPoint;
                    edge.GetPoints(out startPoint, out endPoint);

                    if (MathUtilities.PointWithinLineSegment(startPoint, endPoint, width: 8, point: e.mousePosition))
                    {
                        if ((e.control || e.shift) && !Selection.objects.Contains(GraphController.graph))
                        {
                            int index = ArrayUtility.FindIndex(Selection.objects, (nodeEdge) => nodeEdge == edge);
                            if (index >= 0)
                            {
                                Selection.objects = Selection.objects.WithIndexRemoved(index);
                            }
                            else
                            {
                                Selection.objects = Selection.objects.CopyPushFront(edge);
                            }
                        }
                        else
                        {
                            // UnityShenanigans:
                            // Unlike nodes, Unity's Animation Window deselects other edges when
                            // an edge is selected after it has already been selected.
                            Selection.activeObject = edge;
                        }
                        NodeEditorWindow.instance.Repaint();
                        return;
                    }
                }
            }

            //Nothing was selected and no modifiers were pressed, select the graph.
            if (!e.alt && !e.control && !e.shift)
            {
                Selection.activeObject = GraphController.graph;
                NodeEditorWindow.instance.Repaint();
                return;
            }
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

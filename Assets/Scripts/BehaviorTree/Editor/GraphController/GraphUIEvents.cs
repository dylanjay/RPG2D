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

        /// <summary>
        /// The GraphController this GraphUIEvent is attached to..
        /// </summary>
        private readonly GraphController graphController;

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

        public GraphUIEvents(GraphController graphController, GenericMenu contextMenu)
        {
            this.graphController = graphController;
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
                
                new UIEvent("Pan View")
                {
                    mouseButtons = MouseButtons.Middle,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Pan(e.delta),
                    onEventCancel = (Event e) => Pan(e.mousePosition - dragStartLocation),
                },

                new UIEvent("Pan View")
                {
                    mouseButtons = MouseButtons.Left,
                    modifiers = ModifierKeys.Alt,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Pan(e.delta),
                    onEventCancel = (Event e) => Pan(e.mousePosition - dragStartLocation),
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
                
                new UIEvent("MouseWheel Zoom and Pan")
                {
                    mouseButtons = MouseButtons.None,
                    modifiers = ModifierKeys.None | ModifierKeys.Control,
                    mustHaveAllModifiers = false,
                    eventType = EventType.ScrollWheel,
                    onEventBegin = (Event e) => HandleMouseWheel(e)
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

                new UIEvent("Pan View")
                {
                    mouseButtons = MouseButtons.Middle,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Pan(e.delta),
                    onEventCancel = (Event e) => Pan(e.mousePosition - dragStartLocation),
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
                
                new UIEvent("MouseWheel Zoom and Pan")
                {
                    mouseButtons = MouseButtons.None,
                    modifiers = ModifierKeys.None | ModifierKeys.Control,
                    mustHaveAllModifiers = false,
                    eventType = EventType.ScrollWheel,
                    onEventBegin = (Event e) => HandleMouseWheel(e)
                },

                new UIEvent("Right Click Menu")
                {
                    mouseButtons = MouseButtons.Right,
                    modifiers = ModifierKeys.None,
                    eventType = EventType.MouseUp,
                    onEventExit = (Event e) => ShowNodeContextMenu(e),
                },

                new UIEvent("Create Transition or Pan View")
                {
                    mouseButtons = MouseButtons.Left,
                    modifiers = ModifierKeys.Alt,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => StartTransitionOrPan(e),
                    onEventExit = (Event e) => EndTransitionOrPan(e),
                    onEventUpdate = (Event e) => UpdateTransitionOrPan(e),
                    onRepaint = (Event e) => RepaintTransitionOrPan(e),
                    onEventCancel = (Event e) => CancelTransitionOrPan(e),
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

                new UIEvent("Pan View")
                {
                    mouseButtons = MouseButtons.Left,
                    modifiers = ModifierKeys.Alt,
                    eventType = EventType.MouseDrag,
                    onEventBegin = (Event e) => dragStartLocation = e.mousePosition,
                    onEventUpdate = (Event e) => Pan(e.delta),
                    onEventCancel = (Event e) => Pan(e.mousePosition - dragStartLocation),
                },

                new UIEvent("Delete Edges")
                {
                    mouseButtons = MouseButtons.None,
                    modifiers = ModifierKeys.None,
                    mustHaveAllModifiers = true,
                    eventType = EventType.ValidateCommand,
                    eventCommand = "SoftDelete|Delete",
                    onEventBegin = (Event e) => DeletePressed(e)
                },
            };
        }
        
        private void Pan(Vector2 delta)
        {
            Vector2 scale = graphController.scale;
            delta = new Vector2(delta.x * scale.x, delta.y * scale.y);
            graphController.offset += new Vector2(delta.x , delta.y);
            NodeEditorWindow.instance.Repaint();
        }
        
        private void StartTransitionOrPan(Event e)
        {
            dragStartLocation = e.mousePosition;
            NodeBase selectedNode = Selection.activeObject as NodeBase;
            if (selectedNode == null || !selectedNode.rect.Contains(e.mousePosition))
            {
                return;
            }
            startTransitionNode = selectedNode;
        }

        private void UpdateTransitionOrPan(Event e)
        {
            if (startTransitionNode == null)
            {
                Pan(e.delta);
            }
            else
            {
                NodeEditorWindow.instance.Repaint();
            }
        }

        private void RepaintTransitionOrPan(Event e)
        {
            if (startTransitionNode != null)
            {
                Handles.DrawAAPolyLine(lineTexture, 3, dragStartLocation, e.mousePosition);
                //TODO(mderu): Change this to if (edges are directed for this type of graph)
                if (true)
                {
                    Vector2 line = e.mousePosition - dragStartLocation;
                    Vector2 midpoint = dragStartLocation + line / 2.0f;
                    // Below are the 3 points of the arrow of the directed edge.
                    Vector2 forwardPoint = line.normalized * 7.0f;
                    Vector2 leftPoint = forwardPoint.RotatedBy(120.0f);
                    Vector2 rightPoint = forwardPoint.RotatedBy(240.0f);
                    Handles.DrawAAConvexPolygon(
                        midpoint + forwardPoint,
                        midpoint + leftPoint,
                        midpoint + rightPoint,
                        midpoint + forwardPoint);
                }
            }
        }

        private void HandleMouseWheel(Event e)
        {
            Vector2 scale = graphController.scale;
            Vector2 newScale;
            if (e.control)
            {
                float scaleBy = e.delta.y < 0 ? 2.0f : .5f;
                newScale = new Vector2(Mathf.ClosestPowerOfTwo((int)Mathf.Round(scale.x * 4)) / 4.0f * scaleBy,
                                       Mathf.ClosestPowerOfTwo((int)Mathf.Round(scale.x * 4)) / 4.0f * scaleBy);
            }
            else
            {
                float scaleFactor = .90572366426f * Mathf.Abs(e.delta.y / 3.0f);
                if (e.delta.y > 0)
                {
                    scaleFactor = 1 / scaleFactor;
                }
                newScale = new Vector2(scale.x * scaleFactor, scale.y * scaleFactor);
            }

            // The next 2 if statements below are a hack to get crisp rendering at power-of-two scales.
            if (Mathf.Approximately(Mathf.ClosestPowerOfTwo((int)(newScale.x * 32768)), newScale.x * 32768))
            {
                newScale.x = Mathf.ClosestPowerOfTwo((int)(newScale.x * 32768)) / 32768.0f;
            }
            if (Mathf.Approximately(Mathf.ClosestPowerOfTwo((int)(newScale.y * 32768)), newScale.y * 32768))
            {
                newScale.y = Mathf.ClosestPowerOfTwo((int)(newScale.y * 32768)) / 32768.0f;
            }

            graphController.scale = newScale;

            graphController.offset += new Vector2(e.delta.x, 0);
            graphController.offset = - e.mousePosition.ScaledBy(newScale) +
                                        e.mousePosition.ScaledBy(scale) + graphController.offset;
            NodeEditorWindow.instance.Repaint();
        }

        private void EndTransitionOrPan(Event e)
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

        private void CancelTransitionOrPan(Event e)
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
                NodeEdge nodeEdge = selectedObjects[i] as NodeEdge;
                if (nodeEdge != null)
                {
                    NodeEdge.DestroyEdge(nodeEdge);
                    GraphController.graph.RemoveEdge(nodeEdge);
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

using System.Collections.Generic;
using Benco.Graph;
using UnityEngine;
using UnityEditor;
using ExtensionMethods;
using System.Linq;
using Benco.Utilities;
using Benco.BehaviorTree;

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
        private readonly GraphViewer graphViewer;

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

        /// <summary>
        /// The amount of scroll input sent to the graph that has not resulted in a change in
        /// scale. This is only useful for trackpads, as Unity treats scrolling for
        /// Windows/Linux mice as a integer value. On Windows, the amount is multiplied by the
        /// value set at "Settings > Devices > Mouse > Choose how many lines to scroll each time".
        /// </summary>
        private float mouseScrollRemainder = 0.0f;

        private static readonly Texture2D lineTexture = GUI.skin.GetStyle("selectionRect").active.background;

        private void CreateNode(object sineInfoObject)
        {
            SineInfo sineInfo = (SineInfo)sineInfoObject;
            BehaviorComponent component = BehaviorComponent.CreateComponent(sineInfo.classType);
            NodeUtilities.CreateNode(sineInfo.nodeType,
                                     component,
                                     graphViewer.graph,
                                     graphViewer.GetLastMousePosition());
        }

        public GraphUIEvents(GraphViewer graphController)
        {
            GenericMenu contextMenu = NodeAttributeTags.GetNodeMenu<NodeComposite>(CreateNode);
            this.graphViewer = graphController;
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
            Vector2 scale = graphViewer.scale;
            delta = new Vector2(delta.x * scale.x, delta.y * scale.y);
            graphViewer.offset += new Vector2(delta.x , delta.y);
            graphViewer.Repaint();
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
                graphViewer.Repaint();
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
            Vector2 scale = graphViewer.scale;
            Vector2 newScale;
            mouseScrollRemainder += e.delta.y;
            if (e.control)
            {
                float scaleBy = e.delta.y < 0 ? 2.0f : .5f;
                newScale = new Vector2(Mathf.ClosestPowerOfTwo((int)Mathf.Round(scale.x * 4)) / 4.0f * scaleBy,
                                       Mathf.ClosestPowerOfTwo((int)Mathf.Round(scale.x * 4)) / 4.0f * scaleBy);
            }
            // Only shift zoom amount if we've reached the threshold. Note that we truncate
            // values over 1, because Windows/Linux scroll values are inflated.
            else if (Mathf.Abs(mouseScrollRemainder) > 1)
            {
                // Store the non-trun
                mouseScrollRemainder = Mathf.Repeat(mouseScrollRemainder, 1.0f);
                // Magic Number Explanation:
                // 1.1040894985198974609375f == 2^(1/7): When multiplied 7 times, will give us a
                // 2. In other words, there are exactly 3 scroll steps from a scale of 1 to 2.
                float scaleFactor = 1.1040894985198974609375f;
                // Normally, we would multiply scaleFactor by e.delta, which defines the scroll 
                // wheel's scroll distance, but Unity automatically multiplies this by the OS's
                // line scroll speed, which would make it fail to stop at the crisp values.
                if (e.delta.y > 0)
                {
                    scaleFactor = 1 / scaleFactor;
                }
                newScale = new Vector2(scale.x * scaleFactor, scale.y * scaleFactor);
            }
            // We haven't reached a threshold yet. Do nothing.
            else
            {
                return;
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

            newScale = new Vector2(Mathf.Clamp(newScale.x, .25f, 4.0f),
                                   Mathf.Clamp(newScale.y, .25f, 4.0f));

            graphViewer.scale = newScale;

            graphViewer.offset += new Vector2(e.delta.x, 0);
            graphViewer.offset = - e.mousePosition.ScaledBy(newScale) +
                                        e.mousePosition.ScaledBy(scale) + graphViewer.offset;
            graphViewer.Repaint();
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
            foreach (NodeBase node in graphViewer.graph.nodes)
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

            foreach (NodeBase node in graphViewer.graph.nodes)
            {
                if (node.rect.Contains(e.mousePosition))
                {
                    if ((e.control || e.shift) && !Selection.objects.Contains(graphViewer.graph))
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
                    graphViewer.Repaint();
                    return;
                }
            }

            // Unity by default will not select edges if a right/context-click has been recieved.
            // Don't ask me why, I don't make up the rules.
            if (e.button != 1 && !e.alt)
            {
                foreach (NodeEdge edge in graphViewer.graph.edges)
                {
                    Vector2 startPoint, endPoint;
                    edge.GetPoints(out startPoint, out endPoint);

                    if (MathUtilities.PointWithinLineSegment(startPoint, endPoint, width: 8, point: e.mousePosition))
                    {
                        if ((e.control || e.shift) && !Selection.objects.Contains(graphViewer.graph))
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
                        graphViewer.Repaint();
                        return;
                    }
                }
            }

            //Nothing was selected and no modifiers were pressed, select the graph.
            if (!e.alt && !e.control && !e.shift)
            {
                Selection.activeObject = graphViewer.graph;
                graphViewer.Repaint();
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
            graphViewer.Repaint();
        }

        private void DeletePressed(Event e)
        {
            Object[] selectedObjects = Selection.objects;
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                NodeBase nodeBase = selectedObjects[i] as NodeBase;
                if (nodeBase != null)
                {
                    graphViewer.graph.DeleteNode(nodeBase);
                }
                NodeEdge nodeEdge = selectedObjects[i] as NodeEdge;
                if (nodeEdge != null)
                {
                    NodeEdge.DestroyEdge(nodeEdge);
                    graphViewer.graph.RemoveEdge(nodeEdge);
                }
            }
            graphViewer.Repaint();
        }

        private void StartSelection(Event e)
        {
            firstSelectionPosition = e.mousePosition;
        }

        private void UpdateSelection(Event e)
        {
            secondSelectionPosition = e.mousePosition;
            foreach (NodeBase node in graphViewer.graph.nodes)
            {
                node.isHighlighted = selectionRect.Overlaps(node.rect);
            }
            graphViewer.Repaint();
        }

        private void ExitSelection(Event e, bool canceled)
        {
            bool addedNode = false;
            List<Object> selection = new List<Object>(Selection.objects);
            foreach (NodeBase node in graphViewer.graph.nodes)
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
                    selection.Remove(graphViewer.graph);
                }
                Selection.objects = selection.ToArray();
            }
        }
    }
}

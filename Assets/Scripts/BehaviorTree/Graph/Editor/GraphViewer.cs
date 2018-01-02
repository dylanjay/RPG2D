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
    public class GraphViewer
    {
        private UIEventEngine uiEventEngine = new UIEventEngine();

        public NodeGraph graph { get { return parentWindow.currentGraph; } }
        
        Dictionary<Type, List<UIEvent>> registeredEvents = new Dictionary<Type, List<UIEvent>>();

        public NodeEditorWindow parentWindow { get; private set; }
        public Vector2 offset = new Vector2(0, 0);
        public Vector2 scale = new Vector2(1, 1);

        public GraphViewer(NodeEditorWindow nodeEditorWindow)
        {
            parentWindow = nodeEditorWindow;
            GraphUIEvents graphEvents = new GraphUIEvents(this);
            registeredEvents.Add(typeof(NodeGraph), graphEvents.graphEvents);
            registeredEvents.Add(typeof(NodeBase), graphEvents.nodeEvents);
            registeredEvents.Add(typeof(NodeEdge), graphEvents.edgeEvents);
            Undo.undoRedoPerformed += delegate { parentWindow.Repaint(); };
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
            int snapSize = parentWindow.graphEditorSettings.snapSize;
            float baseValue = 1.0f / Mathf.Log10(Mathf.Max(10, snapSize));
            float zoomPower = (Mathf.Log10(Mathf.Max(viewRect.width, viewRect.height)) * baseValue
                               - Mathf.Log10(scale.x) * baseValue) / Mathf.Log10(17.0f);

            // Split grid draws into 2 zoom groups: low zoom and high zoom
            int lowPower = (int)zoomPower;
            float lowZoomPower = (1 - (zoomPower - (int)zoomPower));
            lowZoomPower *= lowZoomPower;
            int lowLineSpacing = Mathf.RoundToInt(Mathf.Pow(Mathf.Max(10, snapSize), lowPower));
            float highZoomPower = (1 - lowZoomPower);
            int highLineSpacing = lowLineSpacing * 5;
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

        public Rect GetViewRect(NodeBase node)
        {
            Rect nodeRect = node.rect;
            if (parentWindow.graphEditorSettings.snapDimensions)
            {
                int snapSize = parentWindow.graphEditorSettings.snapSize;
                nodeRect.width = Mathf.Ceil(nodeRect.width / snapSize) * snapSize;
                nodeRect.height = Mathf.Ceil(nodeRect.height / snapSize) * snapSize;
            }
            return nodeRect;
        }

        //TODO(mderu): Move this out of GraphController.
        void DrawNode(NodeBase node)
        {
            if (node == null) { return; }
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
            GUI.Box(GetViewRect(node), node.title, nodeStyle);
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
            GetEdgePoints(edge, out startPosition, out endPosition);
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

        public void GetEdgePoints(NodeEdge edge, out Vector2 startPoint, out Vector2 endPoint)
        {
            Rect destinationNodeRect = GetViewRect(edge.destination.node);
            Rect sourceNodeRect = GetViewRect(edge.source.node);
            if (edge.edgeType == EdgeType.Directed)
            {
                // counterClockwiseOffset makes the directed edges offset. This splits the directed
                // edges so they do not align:
                //   ____  /___________ ____
                //  /  |R\ \           /  |R\
                // |  <>  |           |  <>  | (Radius R, <> == center of node)
                //  \____/___________\ \____/
                //                   /
                Vector2 directionVector = destinationNodeRect.center - sourceNodeRect.center;
                // The math below gets the Right vector from the directionVector above.
                Vector2 counterClockwiseOffset = new Vector2(-directionVector.y, directionVector.x);
                counterClockwiseOffset.Normalize();
                counterClockwiseOffset *= NodeEdge.DIRECTED_EDGE_OFFSET_DISTANCE;
                startPoint = sourceNodeRect.center + counterClockwiseOffset;
                endPoint = destinationNodeRect.center + counterClockwiseOffset;
            }
            else
            {
                startPoint = sourceNodeRect.center;
                endPoint = destinationNodeRect.center;
            }
        }

        public Vector2 GetLastMousePosition()
        {
            return uiEventEngine.lastMouseEvent.mousePosition;
        }

        public void Reset()
        {
            uiEventEngine.Reset();
        }

        public void Repaint()
        {
            parentWindow.Repaint();
        }
    }
}

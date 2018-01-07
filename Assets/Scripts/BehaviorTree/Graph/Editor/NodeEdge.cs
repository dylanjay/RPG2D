using Benco.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public enum EdgeType
    {
        Path,
        Directed,
    }

    [System.Serializable]
    public class NodeEdge : ScriptableObject
    {
        protected EdgeType _edgeType = EdgeType.Directed;
        public virtual EdgeType edgeType
        {
            get
            {
                return _edgeType;
            }
            protected set
            {
                _edgeType = value;
            }
        }

        private IEdge _edge;
        public IEdge edge { get { return _edge; } }

        public NodePort source;
        public NodePort destination;
        
        public const float DIRECTED_EDGE_OFFSET_DISTANCE = 6.0f;
        
        public bool isSelected { get { return Selection.Contains(this); } }
        public bool isHighlighted { get; set; }

        public NodePort GetOtherEnd(NodePort currentEnd)
        {
            bool isSource = source == currentEnd;
            Debug.Assert(isSource || currentEnd == destination, "Error: NodePort given did not exist on either side of the edge.");
            return isSource ? destination : source;
        }

        public static bool CanCreateEdge(NodePort srcPort, NodePort dstPort)
        {
            return (srcPort.CanConnectTo(dstPort) && dstPort.CanConnectTo(srcPort));
        }

        public static bool CanCreateEdge(NodeBase src, NodeBase dst)
        {
            return (src.output.CanConnectTo(dst.input) && dst.input.CanConnectTo(dst.output));
        }

        public static NodeEdge CreateEdge(NodePort srcPort, NodePort dstPort, IEdge iEdge = null)
        {
            NodeEdge nodeEdge = CreateInstance<NodeEdge>();

            nodeEdge._edge = iEdge;

            nodeEdge.source = srcPort;
            nodeEdge.destination = dstPort;

            nodeEdge.source.Add(nodeEdge);
            nodeEdge.destination.Add(nodeEdge);

            return nodeEdge;
        }

        public static NodeEdge CreateEdge(NodeBase srcNode, NodeBase dstNode, IEdge iEdge = null)
        {
            NodeEdge nodeEdge = CreateInstance<NodeEdge>();
            Undo.RecordObject(srcNode.parentGraph, "Creating Edge");
            Undo.RecordObject(srcNode.output, "Creating Edge");
            Undo.RecordObject(dstNode.input, "Creating Edge");
            //nodeEdge.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(nodeEdge, srcNode.parentGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            nodeEdge._edge = iEdge;

            nodeEdge.source = srcNode.output;
            nodeEdge.destination = dstNode.input;

            nodeEdge.source.Add(nodeEdge);
            nodeEdge.destination.Add(nodeEdge);

            srcNode.parentGraph.AddEdge(nodeEdge);


            Undo.RegisterCreatedObjectUndo(nodeEdge, "Creating Edge");
            return nodeEdge;
        }

        // TODO(mderu): make this an exposed function on edge.source[.node] / edge.destination[.node]
        public static void DestroyEdge(NodeEdge edge)
        {
            Undo.RecordObject(edge.source.node.parentGraph, "Destroying Edge");
            Undo.RecordObject(edge.source, "Destroying Edge");
            Undo.RegisterCompleteObjectUndo(edge, "Deleting Edge");
            edge.source.node.parentGraph.RemoveEdge(edge);
            edge.source.Remove(edge);
            Undo.DestroyObjectImmediate(edge);
        }

        public void GetEdgePoints(out Vector2 startPoint, out Vector2 endPoint)
        {
            Rect destinationNodeRect = destination.node.rect;
            Rect sourceNodeRect = source.node.rect;
            if (edgeType == EdgeType.Directed)
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
                counterClockwiseOffset *= DIRECTED_EDGE_OFFSET_DISTANCE;
                startPoint = sourceNodeRect.center + counterClockwiseOffset;
                endPoint = destinationNodeRect.center + counterClockwiseOffset;
            }
            else
            {
                startPoint = sourceNodeRect.center;
                endPoint = destinationNodeRect.center;
            }
        }

        public virtual void OnDraw(DrawingSettings settings, Event e)
        {
            Color oldColor = Handles.color;
            if (isSelected || isHighlighted)
            {
                Handles.color = new Color(0.42f, 0.7f, 1.0f);
            }
            Vector2 startPosition, endPosition;
            GetEdgePoints(out startPosition, out endPosition);
            if (edgeType == EdgeType.Directed)
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
    }
}

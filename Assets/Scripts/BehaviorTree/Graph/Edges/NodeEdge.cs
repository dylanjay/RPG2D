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
        public EdgeType edgeType = EdgeType.Directed;

        private IEdge _edge;
        public IEdge edge { get { return _edge; } }

        public NodePort source;
        public NodePort destination;

        private const float DIRECTED_EDGE_OFFSET_DISTANCE = 6.0f;

        public void GetPoints(out Vector2 startPoint, out Vector2 endPoint)
        {
            if (edgeType == EdgeType.Directed)
            {
                // counterClockwiseOffset makes the directed edges offset. This splits the directed
                // edges so they do not align:
                //   ____  /___________ ____
                //  /  |R\ \           /  |R\
                // |  <>  |           |  <>  | (Radius R, <> == center of node)
                //  \____/___________\ \____/
                //                   /
                Vector2 directionVector = destination.node.rect.center - source.node.rect.center;
                // The math below gets the Right vector from the directionVector above.
                Vector2 counterClockwiseOffset = new Vector2(-directionVector.y, directionVector.x);
                counterClockwiseOffset.Normalize();
                counterClockwiseOffset *= DIRECTED_EDGE_OFFSET_DISTANCE;
                startPoint = source.node.rect.center + counterClockwiseOffset;
                endPoint = destination.node.rect.center + counterClockwiseOffset;
            }
            else
            {
                startPoint = source.node.rect.center;
                endPoint = destination.node.rect.center;
            }
        }

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
    }
}

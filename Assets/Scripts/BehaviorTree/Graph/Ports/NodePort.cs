using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    //TODO: Prevent nodes from connecting to themselves for trees.
    public abstract class NodePort : ScriptableObject
    {
        /// <summary>
        /// The node the NodePort belongs to.
        /// </summary>
        public NodeBase node = null;

        public void Init(NodeBase node)
        {
            this.node = node;
        }

        public abstract bool CanConnectTo(NodePort port);
        
        internal virtual void Add(NodeEdge nodeEdge)
        {
            Undo.RecordObject(nodeEdge, "Adding Edge");
            Undo.RecordObject(this, "Adding Edge");
        }
        

        internal virtual bool Remove(NodeEdge nodeEdge)
        {
            Undo.RecordObject(nodeEdge, "Adding Edge");
            Undo.RecordObject(this, "Adding Edge");
            return false;
        }

        internal void DeleteAllEdges()
        {
            List<NodeEdge> edges = new List<NodeEdge>(this.edges);

            Undo.RecordObject(this, "Delete NodePort Edges");
            foreach (NodeEdge edge in edges)
            {
                Undo.RecordObject(edge.GetOtherEnd(this), "Remove Edge from Other End");
                Remove(edge);
                node.parentGraph.RemoveEdge(edge);
                Undo.DestroyObjectImmediate(edge);
            }
        }

        /// <summary>
        /// The node the port is connected to. The first if there are multiple.
        /// </summary>
        public abstract NodeBase connectedNode { get; }

        public abstract int connectionCount { get; }

        public abstract IEnumerable<NodeEdge> edges { get; }

        public IEnumerable<NodePort> ports
        {
            get
            {
                foreach (NodeEdge edge in edges)
                {
                    yield return edge.GetOtherEnd(this);
                }
            }
        }

        public IEnumerable<NodeBase> nodes
        {
            get
            {
                foreach (NodePort port in ports)
                {
                    yield return port.node;
                }
            }
        }

    }
}

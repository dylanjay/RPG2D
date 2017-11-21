using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.Graph
{
    public class MultiPort : NodePort
    {
        [SerializeField]
        List<NodeEdge> connections = new List<NodeEdge>();

        public override NodeBase connectedNode
        {
            get
            {
                return connections.Count > 0 ? connections[0].GetOtherEnd(this).node : null;
            }
        }

        public override int connectionCount
        {
            get
            {
                return connections.Count;
            }
        }

        public override bool CanConnectTo(NodePort port)
        {
            return true;
        }

        internal override void Add(NodeEdge edge)
        {
            base.Add(edge);
            connections.Add(edge);
        }

        internal override bool Remove(NodeEdge edge)
        {
            base.Remove(edge);
            bool removed = connections.Remove(edge);
            if (removed)
            {
                edge.GetOtherEnd(this).Remove(edge);
            }
            return removed;
        }

        public override IEnumerable<NodeEdge> edges
        {
            get
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    yield return connections[i];
                }
            }
        }
    }
}


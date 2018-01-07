using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.Graph
{
    public class ClosedPort : NodePort
    {
        public override NodeBase connectedNode
        {
            get
            {
                return null;
            }
        }

        public override int connectionCount
        {
            get
            {
                return 0;
            }
        }

        public override bool CanConnectTo(NodePort port)
        {
            return false;
        }

        internal override void Add(NodeEdge edge) { }

        internal override bool Remove(NodeEdge edge)
        {
            return false;
        }

        public override IEnumerable<NodeEdge> edges
        {
            get
            {
                yield break;
            }
        }
    }

}

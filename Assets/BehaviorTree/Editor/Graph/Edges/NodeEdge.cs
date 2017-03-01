using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public enum EdgeType
    {
        Path,
        Directed,
    }

    [System.Serializable]
    public class NodeEdge
    {
        public EdgeType edgeType = EdgeType.Directed;

        private Edge _edge;
        public Edge edge { get { return _edge; } }

        public NodeBase nodeA;
        public NodeBase nodeB;
    }
}

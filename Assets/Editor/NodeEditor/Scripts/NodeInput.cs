using UnityEngine;
using System;

namespace Benco.BehaviorTree.TreeEditor
{
    [Serializable]
    public class NodeInput
    {
        public bool isOccupied = false;
        public NodeBase parentNode;
        public Vector2 position;
    }
}

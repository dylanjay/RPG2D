using UnityEngine;
using System;

namespace Benco.BehaviorTree.TreeEditor
{
    [Serializable]
    public class NodeInput
    {
        private NodeBase node;

        public bool isOccupied { get { return _parentNode != null; } }

        private NodeBase _parentNode;
        public NodeBase parentNode
        {
            get
            {
                return _parentNode;
            }
            set
            {
                _parentNode = value;
            }
        }
        public Vector2 position;
    }
}

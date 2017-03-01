using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public abstract class NodePort : ScriptableObject, IEnumerable<NodePort>
    {
        [SerializeField]
        protected NodeBase _fromNode;
        public NodeBase node { get; private set; }

        public NodePort(NodeBase node)
        {
            this.node = node;
        }

        public abstract void Add(NodePort connector);
        public abstract bool Remove(NodePort connector);
        public abstract NodeBase connectedNode { get; }

        public abstract IEnumerator<NodePort> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return GetEnumerator();
        }
    }
}

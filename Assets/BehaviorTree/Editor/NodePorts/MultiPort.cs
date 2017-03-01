using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public class MultiPort : NodePort
    {
        [SerializeField]
        List<NodePort> connections;

        public MultiPort(NodeBase node) : base(node) { }

        public override NodeBase connectedNode
        {
            get
            {
                return connections.Count > 0 ? connections[0].node : null;
            }
        }

        public override void Add(NodePort connector)
        {
            connections.Add(connector);
        }

        public override bool Remove(NodePort connector)
        {
            int foundIndex = connections.FindIndex(x => x == connector);
            if (foundIndex < 0)
            {
                return false;
            }
            connections.RemoveAt(foundIndex);
            connector.Remove(connector);
            return true;
        }

        public override IEnumerator<NodePort> GetEnumerator()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                yield return connections[i];
            }
        }
    }
}


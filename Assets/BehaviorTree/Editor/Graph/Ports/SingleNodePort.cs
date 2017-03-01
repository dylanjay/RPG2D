using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public class ConnectorSingle : NodePort
    {
        [SerializeField]
        NodePort connection;

        public ConnectorSingle(NodeBase node) : base(node) { }

        public override NodeBase connectedNode
        {
            get
            {
                return connection.node;
            }
        }

        public override void Add(NodePort connector)
        {
            connection = connector;
        }
        
        public override bool Remove(NodePort connector)
        {
            if (connector != connection)
            {
                return false;
            }
            connection = null;
            connector.Remove(connector);
            return true;
        }

        public override IEnumerator<NodePort> GetEnumerator()
        {
            yield return connection;
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class SingleNodePort : NodePort
    {
        [SerializeField]
        NodeEdge connection;
        
        public override NodeBase connectedNode
        {
            get
            {
                return connection.GetOtherEnd(this).node;
            }
        }

        public override int connectionCount
        {
            get
            {
                return connection != null ? 1 : 0;
            }
        }
        
        public override bool CanConnectTo(NodePort port)
        {
            return port != this;
        }

        /// <summary>
        /// Gets called from NodeEdge.CreateEdge(src, dst).
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        internal override void Add(NodeEdge edge)
        {
            base.Add(edge);
            if (connection != null)
            {
                NodeEdge.DestroyEdge(connection);
            }
            connection = edge;
        }
        
        /// <summary>
        /// Gets called implicitly from DestroyImmmediate(edge).
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        internal override bool Remove(NodeEdge edge)
        {
            base.Remove(edge);
            if (edge != connection)
            {
                return false;
            }
            connection = null;
            edge.GetOtherEnd(this).Remove(edge);
            return true;
        }

        public override IEnumerable<NodeEdge> edges
        {
            get
            {
                if (connection != null)
                {
                    yield return connection;
                }
            }
        }
    }
}


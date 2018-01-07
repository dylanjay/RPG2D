using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using Type = System.Type;

namespace Benco.Graph
{
    public abstract class NodeGraph : ScriptableObject
    {
        public enum DeleteReturnType
        {
            Success,
            DoesNotExist,
            InternalFailure,
        }

        [SerializeField]
        protected List<NodeBase> nodeList = new List<NodeBase>();
        public IEnumerable<NodeBase> nodes { get { return nodeList.AsEnumerable(); } }
        public int nodeCount { get { return nodeList.Count; } }

        [SerializeField]
        protected List<NodeEdge> edgeList = new List<NodeEdge>();
        public IEnumerable<NodeEdge> edges { get { return edgeList.AsEnumerable(); } }
        public int edgeCount { get { return edgeList.Count; } }

        public virtual void AddNode(NodeBase node)
        {
            Undo.RecordObject(this, "Adding Node");
            if (nodeList.Contains(node))
            {
                Debug.LogWarning("Already contains node " + node.name);
                return;
            }
            if (nodeList.Count == 0)
            {
                node.name = "Node " + default(int);
            }
            else
            {
                //A complete hack. Strips "Node " from the previously added node name and adds 1 to it.
                //i.e: nodeList[last].name = "Node 5" => int("5") + 1
                node.name = "Node " + (int.Parse(nodeList[nodeList.Count - 1].name.Substring(5)) + 1);
            }
            nodeList.Add(node);
        }

        /// <summary>
        /// Calculates whether or not <paramref name="node"/> can be deleted.
        /// </summary>
        /// <returns>A <see cref="DeleteReturnType"/> with a meaningful message.</returns>
        public virtual DeleteReturnType CanDeleteNode(NodeBase node)
        {
            return DeleteReturnType.Success;
        }

        /// <summary>
        /// Defines what action should be performed when calling <see cref="DeleteNode(NodeBase)"/>.
        /// </summary>
        /// <returns><see cref="DeleteReturnType.Success"/> if deletion is possible/permitted.</returns>
        public virtual DeleteReturnType OnDeleteNode() { return DeleteReturnType.Success; }

        /// <summary>
        /// Delete a node form the graph.
        /// </summary>
        /// <param name="node">The node to delete.</param>
        /// <returns>
        /// Whether or not the node was successfully deleted. Usually returns
        /// false when the node didn't exist within the graph.
        /// </returns>
        /// <remarks>Do not return the base version of this. Handle the return in the child class.</remarks>
        public virtual DeleteReturnType DeleteNode(NodeBase node)
        {
            if (!nodeList.Contains(node))
            {
                return DeleteReturnType.DoesNotExist;
            }
            DeleteReturnType deleteReturnType = CanDeleteNode(node);
            if (deleteReturnType != DeleteReturnType.Success)
            {
                return deleteReturnType;
            }

            Undo.RecordObject(this, "Deleting Node");
            OnDeleteNode();
            // Remove the root if this node was root.
            if (root == node)
            {
                root = null;
            }
            node.input.DeleteAllEdges();
            node.output.DeleteAllEdges();

            NodePort input = node.input;
            NodePort output = node.output;
            Undo.RegisterCompleteObjectUndo(node, "Deleting Node");
            Undo.RegisterCompleteObjectUndo(node.input, "Deleting Node");
            Undo.RegisterCompleteObjectUndo(node.output, "Deleting Node");
            node.input = null;
            node.output = null;
            Undo.DestroyObjectImmediate(node);
            Undo.DestroyObjectImmediate(input);
            Undo.DestroyObjectImmediate(output);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return DeleteReturnType.Success;
        }


        public void AddEdge(NodeEdge edge)
        {
            edgeList.Add(edge);
        }

        public bool RemoveEdge(NodeEdge edge)
        {
            return edgeList.Remove(edge);
        }

        [SerializeField]
        private NodeBase _root;
        public virtual NodeBase root { get { return _root; } set { _root = value; } }

        public virtual void Initialize() { }


        /// <summary>
        /// A function similar to <see cref="MonoBehaviour.Update()"/>, but for the Editor.
        /// </summary>
        /// <param name="e"></param>
        public virtual void UpdateGraph(Event e) { }

        /// <summary>
        /// The function call to create the NodeGraph's Asset in the Project View.
        /// </summary>
        public void CreateAsset()
        {
            //TODO: Prevent characters that cannot be in a file name from being typed in.
            AssetDatabase.CreateAsset(this, @"Assets/Resources/BehaviorTrees/" + name + ".asset");
            OnCreateAsset();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// A 
        /// </summary>
        public virtual void OnCreateAsset() { }
    }
}

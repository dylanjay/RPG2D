using UnityEngine;
using UnityEditor;
using Type = System.Type;
using System.Collections.Generic;

namespace Benco.Graph
{
    public abstract class NodeBase : ScriptableObject
    {
        public string title;
        public string description;

        /// <summary>
        /// The Rect drawn to display the Node in the Node Editor.
        /// </summary>
        public Rect rect;

        /// <summary>
        /// The graph the node belongs to.
        /// </summary>
        public NodeGraph parentGraph;

        public bool isSelected { get { return Selection.Contains(this); } }
        public bool isHighlighted { get; set; }

        public NodePort output = null;
        public NodePort input = null;
        
        /// <summary>
        /// Returns the list of all connections between two given nodes.
        /// </summary>
        /// <returns></returns>
        public static List<NodeEdge> GetEdgesBetween(NodeBase first, NodeBase second)
        {
            List<NodeEdge> nodeEdges = new List<NodeEdge>();
            foreach (NodeEdge edge in first.output.edges)
            {
                if (edge.destination == second)
                {
                    nodeEdges.Add(edge);
                }
            }

            foreach (NodeEdge edge in second.output.edges)
            {
                if (edge.source == first)
                {
                    nodeEdges.Add(edge);
                }
            }
            return nodeEdges;
        }

        /// <summary>
        /// Returns whether or not any edge exists between the two given nodes.
        /// Order of arguments does not matter.
        /// </summary>
        /// <returns>True if an edge exists between two given nodes.</returns>
        public static bool AreConnected(NodeBase first, NodeBase second)
        {
            foreach (NodeBase node in first.output.nodes)
            {
                if (node == second)
                {
                    return true;
                }
            }

            foreach (NodeBase node in second.output.nodes)
            {
                if (node == first)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the edge going from this node to another. Null if no node exists.
        /// This function respects direction of the edges. To check for any edge one
        /// way or the other, <see cref="GetEdgesBetween(NodeBase, NodeBase)"/>.
        /// </summary>
        /// <returns>The edge going from this node to another. Null if no node exists.
        /// </returns>
        NodeEdge EdgeTo(NodeBase otherNode)
        {
            if (output)
            {
                foreach (NodeEdge edge in output.edges)
                {
                    if (edge.destination == otherNode)
                    {
                        return edge;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// True if an edge exists between this node and the other.
        /// This function respects the direction of the edges. To check if any edge exists between
        /// the nodes, <see cref="AreConnected(NodeBase, NodeBase)"/>.
        /// </summary>
        /// <returns>The edge going from this node to another. Null if no node exists.</returns>
        bool HasEdgeTo(NodeBase otherNode)
        {
            return EdgeTo(otherNode) != null;
        }

        public static NodeBase CreateNode(Type t, NodeGraph nodeGraph, Vector2 position)
        {
            NodeBase nodeBase = (NodeBase)ScriptableObject.CreateInstance(t);
            nodeBase.hideFlags = HideFlags.HideInHierarchy;
            nodeBase.title = nodeBase.name = nodeBase.GetType().ToString().Substring(4);

            nodeBase.InternalInitialize();
            nodeBase.rect.x = position.x;
            nodeBase.rect.y = position.y;
            nodeBase.parentGraph = nodeGraph;
            nodeGraph.AddNode(nodeBase);

            AssetDatabase.AddObjectToAsset(nodeBase, nodeGraph);
            AssetDatabase.AddObjectToAsset(nodeBase.input, nodeGraph);
            AssetDatabase.AddObjectToAsset(nodeBase.output, nodeGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Undo.RegisterCompleteObjectUndo(nodeBase, "Create Node");
            return nodeBase;
        }

        /// <summary>
        /// The initialize that is internal to NodeBase. Calls Initialize() and makes assertions.
        /// </summary>
        internal void InternalInitialize()
        {
            rect = new Rect(10, 10, 150, 35);
            Initialize();
            Debug.Assert(input != null, "The input port must be initialized in Initialize().");
            Debug.Assert(output != null, "The output port must be initialized in Initialize().");
        }

        /// <summary>
        /// The function to initialize the node. This function must initialize the input and output NodePorts.
        /// </summary>
        protected abstract void Initialize();

        public virtual void UpdateNode(Event e) { }
        
        /// <summary>
        /// Get the context ("right-click") menu for this node.
        /// </summary>
        /// <param name="e">The event that caused the context menu to trigger.</param>
        /// <returns>A GenericMenu with default menu items.</returns>
        public virtual GenericMenu GetContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();

            GUIContent rootToggle = new GUIContent(parentGraph.root == this ? "Un-set as Root" : "Set as Root");
            menu.AddItem(rootToggle, false, ToggleRoot);
            menu.AddDisabledItem(new GUIContent("Make Transition with &Drag"));
            return menu;
        }

        public void ShowContextMenu(Event e)
        {
            GetContextMenu(e).ShowAsContext();
        }

        /// <summary>
        /// Toggles whether or not this node is the root of the graph.
        /// </summary>
        private void ToggleRoot()
        {
            NodeBase nodeBase = parentGraph.root == this ? null : this;
            parentGraph.root = nodeBase;
        }

        /// <summary>
        /// Calls <see cref="NodeGraph.DeleteNode(NodeBase)"/> on on its parent with itself as the parameter.
        /// </summary>
        void Delete()
        {
            parentGraph.DeleteNode(this);
        }

        private Rect GetBounds(DrawingSettings settings)
        {
            if (settings.snapDimensions && settings.snapSize > 1)
            {
                Rect rect = this.rect;
                int snapSize = settings.snapSize;
                rect.width = Mathf.Ceil(rect.width / snapSize) * snapSize;
                rect.height = Mathf.Ceil(rect.height / snapSize) * snapSize;
                return rect;
            }
            else
            {
                return new Rect(rect.position, new Vector2(150, 35));
            }
        }

        public virtual void OnDraw(DrawingSettings drawingSettings, Event e)
        {
            GUIStyle nodeStyle;

            if (isSelected || isHighlighted)
            {
                if (parentGraph.root == this)
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 3 on");
                }
                else
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 0 on");
                }
            }
            else
            {
                if (parentGraph.root == this)
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 3");
                }
                else
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 0");
                }
            }
            rect = GetBounds(drawingSettings);
            GUI.Box(rect, title, nodeStyle);
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using Type = System.Type;

namespace Benco.BehaviorTree.TreeEditor
{
    public class NodeGraph : ScriptableObject
    {
        public string graphName = "New Graph";
        public string description = "";
        
        [SerializeField]
        private List<NodeBase> nodeList = new List<NodeBase>();

        public int NodeCount { get { return nodeList.Count; } }

        public IEnumerable<NodeBase> nodes { get { return nodeList.AsEnumerable(); } }

        [SerializeField]
        public SharedVariableCollection sharedVariableCollection;

        [HideInInspector]
        public NodeBase selectedNode;
        [HideInInspector]
        public bool wantsConnection;
        [HideInInspector]
        public NodeBase connectionNode;
        private NodeBase disconnectNode;
        [HideInInspector]
        public bool showProperties;
        public NodeBase rootNode;

        private int newSharedVariableTypeIndex;
        private string newSharedVariableName;

        void OnEnable()
        {
            if (sharedVariableCollection == null)
            {
                sharedVariableCollection = new SharedVariableCollection(this);
            }
            //sharedVariableCollection.OnEnable();
        }

        public void Initialize()
        {
            if (nodeList.Any())
            {
                foreach (NodeBase node in nodeList)
                {
                    node.Initialize();
                }
            }
        }


        public void UpdateGraph(Event e)
        {
            if (selectedNode == null)
            {
                showProperties = false;
            }

            else
            {
                showProperties = true;
            }

            if (nodeList.Any())
            {
                //If somehow a node becomes null, remove it.
                nodeList.RemoveAll(n => n == null);

                foreach (NodeBase node in nodeList)
                {
                    node.UpdateNode(e);
                }
            }
        }

#if UNITY_EDITOR
        public void UpdateGraphGUI(Event e, Rect viewRect)
        {
            ProcessEvents(e, viewRect);
            if (nodeList.Any())
            {
                foreach (NodeBase node in nodeList)
                {
                    node.UpdateNodeGUI(e, viewRect);
                }
            }

            if (wantsConnection)
            {
                if (connectionNode != null)
                {
                    DrawConnectionToMouse(e.mousePosition);
                }
            }
        }

        void ProcessEvents(Event e, Rect viewRect)
        {
            if (e.isMouse && viewRect.Contains(e.mousePosition))
            {
                if (e.button == 0)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        Object selection = this;
                        if (selectedNode != null)
                        {
                            selectedNode.isSelected = false;
                        }
                        selectedNode = null;
                        foreach (NodeBase node in nodeList)
                        {
                            if (node.nodeRect.Contains(e.mousePosition))
                            {
                                selectedNode = node;
                                selection = node;
                                node.isSelected = true;
                                break;
                            }

                            else if (node.output != null)
                            {
                                if (node.outputRect.Contains(e.mousePosition))
                                {
                                    connectionNode = node;
                                    wantsConnection = true;
                                }

                                foreach (NodeBase childNode in node.output.childNodes)
                                {
                                    if (childNode.inputRect.Contains(e.mousePosition))
                                    {
                                        connectionNode = node;
                                        disconnectNode = childNode;
                                        wantsConnection = true;
                                    }
                                }
                            }
                        }
                        Selection.activeObject = selection;
                    }
                    else if (e.type == EventType.MouseUp)
                    {
                        bool hitNode = false;
                        if (wantsConnection)
                        {
                            foreach (NodeBase node in nodeList)
                            {
                                if (node.input != null)
                                {
                                    if (node.inputRect.Contains(e.mousePosition))
                                    {
                                        hitNode = true;
                                        if (node != connectionNode)
                                        {
                                            if (selectedNode != null)
                                            {
                                                selectedNode.isSelected = false;
                                            }
                                            selectedNode = null;

                                            if (node.input.parentNode == connectionNode)
                                            {
                                                DisconnectNodes(connectionNode, node);
                                            }

                                            if (disconnectNode != null)
                                            {
                                                DisconnectNodes(connectionNode, disconnectNode);
                                            }

                                            if (connectionNode.GetType() == typeof(NodeDecorator) && connectionNode.output.childNodes.Any())
                                            {
                                                DisconnectNodes(connectionNode, connectionNode.output.childNodes[0]);
                                            }

                                            node.input.parentNode = connectionNode;
                                            node.input.isOccupied = node.input.parentNode != null;
                                            connectionNode.output.childNodes.Add(node);
                                            node.input.parentNode.output = connectionNode.output;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!hitNode && disconnectNode != null)
                            {
                                DisconnectNodes(connectionNode, disconnectNode);
                            }
                        }
                        wantsConnection = false;
                        connectionNode = null;
                        disconnectNode = null;
                    }
                }
            }

            if (e.keyCode == KeyCode.Delete && selectedNode != null)
            {
                DeleteNode(selectedNode);
                selectedNode = null;
            }
        }

        public void Reset()
        {
            selectedNode = null;
            wantsConnection = false;
            connectionNode = null;
            disconnectNode = null;
            showProperties = false;
        }

        void DisconnectNodes(NodeBase male, NodeBase female)
        {
            male.output.childNodes.Remove(female);
            female.input.parentNode = null;
            female.input.isOccupied = false;
        }

        void DrawConnectionToMouse(Vector2 position)
        {
            Vector3 start = connectionNode.output.position;
            Vector3 end = position;
            Vector2 startTangent = new Vector2();
            Vector2 endTangent = new Vector2();
            Color color = Color.gray;
            color = new Color(1, 0.5f, 0);
            float offset = Mathf.Abs(start.x - end.x) / 1.75f;
            if (position.x < start.x)
            {
                startTangent = new Vector2(start.x - offset, start.y);
                endTangent = new Vector2(end.x + offset, end.y);
            }
            else
            {
                startTangent = new Vector2(start.x + offset, start.y);
                endTangent = new Vector2(end.x - offset, end.y);
            }

            Handles.BeginGUI();
            {
                Handles.color = Color.white;
                Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 2);
            }
            Handles.EndGUI();
        }

        public void DeleteNode(NodeBase node)
        {
            if (rootNode == node)
            {
                rootNode = null;
            }

            if (node.output != null)
            {
                foreach (NodeBase child in node.output.childNodes)
                {
                    if (child.input.parentNode == node)
                    {
                        child.input.parentNode = null;
                        child.input.isOccupied = false;
                        node.output.childNodes.Remove(child);
                    }
                }
            }
            nodeList.Remove(node);
            if (node.behaviorComponent != null)
            {
                DestroyImmediate(node.behaviorComponent, true);
            }
            DestroyImmediate(node, true);
            AssetDatabase.Refresh();
        }
#endif

        bool IsConnected(NodeBase first, NodeBase second)
        {
            if (first.output != null)
            {
                foreach (NodeBase node in first.output.childNodes)
                {
                    if (node == second)
                    {
                        return true;
                    }
                }
            }

            if (second.output != null)
            {
                foreach (NodeBase node in second.output.childNodes)
                {
                    if (node == first)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddNode(NodeBase node)
        {
            if (nodeList.Contains(node))
            {
                Debug.Log("Already contains node " + node.name);
            }
            if (nodeList.Count == 0)
            {
                node.name = "Node " + default(int);
            }
            else
            {
                node.name = "Node " + (int.Parse(nodeList[nodeList.Count - 1].name.Substring(5)) + 1);
            }
            nodeList.Add(node);
        }

        public void AddBehavior(NodeBase node)
        {
            sharedVariableCollection.AddBehavior(node);
        }

        public void RemoveBehavior(NodeBase node)
        {
            sharedVariableCollection.RemoveBehavior(node);
        }

        public GUIContent[] GetDropdownOptions(Type type)
        {
            return sharedVariableCollection.GetDropdownOptions(type);
        }

        public void SetReference(NodeBase node, string fieldName, string prevOption, string currentOption)
        {
            sharedVariableCollection.SetReference(node, fieldName, prevOption, currentOption);
        }

        /// <summary>
        /// Clears a reference that corresponds to the given parameters.
        /// </summary>
        /// <param name="node">The Node that is handling the behavior.</param>
        /// <param name="fieldName">The name of the field being cleared within the behavior.</param>
        /// <param name="sharedVariableName">The previous option (SharedVariable name) selected in the dropdown menu.</param>
        public void RemoveReference(NodeBase node, string fieldName, string sharedVariableName)
        {
            sharedVariableCollection.RemoveReference(node, fieldName, sharedVariableName);
        }

        public void CreateAsset()
        {
            //TODO: Prevent characters that cannot be in a file name from being typed in.
            AssetDatabase.CreateAsset(this, @"Assets/Resources/BehaviorTrees/" + graphName + ".asset");
            sharedVariableCollection.OnCreateAsset();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ShowValueEditor(Type type, object sharedVariable, GUIContent guiContent)
        {
            object value = sharedVariable.GetType().GetField("value").GetValue(sharedVariable);
            System.Func<object, object> func;
            if (_Fields.TryGetValue(type, out func))
            {
                object newValue = func.Invoke(value);

                if (!value.Equals(newValue))
                {
                    Debug.Log("HEre");
                    sharedVariable.GetType().GetField("value").SetValue(sharedVariable, newValue);
                }
            }
            else if (typeof(Object).IsAssignableFrom(type))
            {
                func = _Fields[typeof(Object)];
                object newValue = func.Invoke(value);

                if (!value.Equals(newValue))
                {
                    sharedVariable.GetType().GetField("value").SetValue(sharedVariable, newValue);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Type " + type + " is unsupported.");
            }
        }

        private static readonly Dictionary<Type, System.Func<object, object>> _Fields =
            new Dictionary<Type, System.Func<object, object>>()
            {
            { typeof(int),      value => EditorGUILayout.DelayedIntField((int)value)},
            { typeof(float),    value => EditorGUILayout.DelayedFloatField((float)value) },
            { typeof(double),   value => EditorGUILayout.DelayedDoubleField((float)value) },
            { typeof(string),   value => EditorGUILayout.DelayedTextField((string)value) },
            { typeof(bool),     value => EditorGUILayout.Toggle((bool)value) },
            { typeof(Vector2),  value => EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value) },
            { typeof(Vector3),  value => EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value) },
            { typeof(Vector4),  value => EditorGUILayout.Vector4Field(GUIContent.none, (Vector3)value) },
            { typeof(Bounds),   value => EditorGUILayout.BoundsField((Bounds)value) },
            { typeof(Rect),     value => EditorGUILayout.RectField((Rect)value) },
            { typeof(Object),   value => EditorGUILayout.ObjectField(GUIContent.none, (Object)value, value.GetType(), false)},
            };
    }
}

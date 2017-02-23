using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.ObjectModel;
using Type = System.Type;
using System.Collections.Generic;

namespace Benco.BehaviorTree.TreeEditor
{
    public abstract class NodeBase : ScriptableObject
    {
        public string title;
        public string description;

        public Rect nodeRect;
        public NodeGraph parentGraph;
        [HideInInspector]
        public bool isSelected = false;
        public NodeOutput output;
        public NodeInput input;
        [HideInInspector]
        public Rect outputRect;
        [HideInInspector]
        public Rect inputRect;
        [HideInInspector]
        public GUISkin nodeSkin;

        public BehaviorComponent behaviorComponent;

        [System.Serializable]
        protected class ChoiceDictionary : SerializableDictionary<string, SharedVariable> { }
        /// <summary>
        /// Key: The name of the field
        /// Value: SGUIContent dropdown option
        /// </summary>
        [SerializeField]
        protected ChoiceDictionary choices = new ChoiceDictionary();


        public static NodeBase CreateNode(Type t, NodeGraph nodeGraph, Vector2 position)
        {
            NodeBase nodeBase = (NodeBase)ScriptableObject.CreateInstance(t);
            //nodeBase.hideFlags = HideFlags.HideInHierarchy;
            nodeBase.title = nodeBase.name = nodeBase.GetType().ToString().Substring(4);

            nodeBase.Initialize();
            nodeBase.nodeRect.x = position.x;
            nodeBase.nodeRect.y = position.y;
            nodeBase.parentGraph = nodeGraph;
            nodeGraph.AddNode(nodeBase);

            AssetDatabase.AddObjectToAsset(nodeBase, nodeGraph);
            nodeBase.hideFlags = HideFlags.None;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return nodeBase;
        }


        public void BuildTree()
        {
            if (behaviorComponent.GetType().BaseType == typeof(BehaviorComposite))
            {
                BehaviorComposite composite = behaviorComponent as BehaviorComposite;
                BehaviorComponent[] childComponents = new BehaviorComponent[output.childNodes.Count];
                for (int i = 0; i < output.childNodes.Count; i++)
                {
                    childComponents[i] = output.childNodes[i].behaviorComponent;
                }
                composite.Initialize(title, childComponents);
            }
            else if (behaviorComponent.GetType().BaseType == typeof(BehaviorDecorator))
            {
                BehaviorDecorator decorator = behaviorComponent as BehaviorDecorator;
                decorator.Initialize(title, output.childNodes[0].behaviorComponent);
            }
        }

        public virtual void Reset()
        {
            if (parentGraph != null)
            {
                if (parentGraph.rootNode != null)
                {
                    if (parentGraph.rootNode == this)
                    {
                        input = null;
                    }
                }
            }
        }

        public void SaveBehavior()
        {
            if (behaviorComponent != null && !AssetDatabase.Contains(behaviorComponent))
            {
                AssetDatabase.AddObjectToAsset(behaviorComponent, parentGraph);
            }
        }

        public virtual void Initialize()
        {
            GetEditorSkin();
            if (output != null)
            {
                outputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y + nodeRect.height, 24, 18);
            }

            if (input != null)
            {
                inputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y - 18, 24, 18);
            }
            //hideFlags = HideFlags.HideInHierarchy;
        }

        public abstract GUIContent[] GetAllBehaviorOptions();
        public abstract ReadOnlyCollection<Type> GetAllBehaviorTypes();

        public virtual void UpdateNode(Event e)
        {
        }

#if UNITY_EDITOR
        public virtual void UpdateNodeGUI(Event e, Rect viewRect)
        {
            //TODO : switch to reset function
            if (parentGraph.rootNode == this)
            {
                input = null;
            }
            if (nodeSkin == null)
            {
                GetEditorSkin();
            }
            GUIStyle nodeStyle;
            if (isSelected)
            {
                nodeStyle = nodeSkin.GetStyle("NodeSelected");
            }
            else if (parentGraph.rootNode == this)
            {
                nodeStyle = nodeSkin.GetStyle("NodeRoot");
            }
            else
            {
                nodeStyle = nodeSkin.GetStyle("NodeDefault");
            }
            GUI.Box(nodeRect, title, nodeStyle);

            if (output != null)
            {
                outputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y + nodeRect.height, 24, 18);
                GUI.Box(outputRect, "", nodeSkin.GetStyle("NodeConnectionBackBelow"));
                output.position = new Vector2(outputRect.position.x + outputRect.width / 2, outputRect.position.y + outputRect.height / 2);
            }

            if (input != null)
            {
                inputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y - 18, 24, 18);
                GUI.Box(inputRect, "", nodeSkin.GetStyle("NodeConnectionBackAbove"));
                input.position = new Vector2(inputRect.position.x + inputRect.width / 2, inputRect.position.y + inputRect.height / 2);
            }

            DrawConnections();
            ProcessEvents(e, viewRect);
            EditorUtility.SetDirty(this);
        }

        void DrawConnections()
        {
            if (input == null) { return; }
            if (input.parentNode == null) { return; }

            Vector3 start = input.parentNode.output.position;
            Vector3 end = input.position;
            Vector2 startTangent = new Vector2();
            Vector2 endTangent = new Vector2();
            Color color = Color.gray;
            if (isSelected || input.parentNode.isSelected)
            {
                color = new Color(1, 0.5f, 0);
            }
            float offset = Mathf.Abs(start.x - end.x) / 1.75f;
            if (input.position.x < start.x)
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

        public virtual void DrawNodeProperties()
        {

        }

        public virtual void DrawNodeHelp()
        {

        }
#endif

        public virtual void ProcessEvents(Event e, Rect viewRect)
        {
            if (viewRect.Contains(e.mousePosition))
            {
                if (isSelected && e.button == 0)
                {
                    if (e.type == EventType.MouseDrag)
                    {
                        nodeRect.x += e.delta.x;
                        nodeRect.y += e.delta.y;
                    }
                    NodeEditorWindow.instance.Repaint();
                }
                else if (e.type == EventType.MouseDown && e.button == 1 && nodeRect.Contains(e.mousePosition))
                {
                    ProcessContextMenu(e);
                }
                else
                {
                    if (e.type == EventType.MouseDrag && e.button == 2)
                    {
                        foreach (NodeBase node in parentGraph.nodes)
                        {
                            nodeRect.x += e.delta.x / parentGraph.NodeCount;
                            nodeRect.y += e.delta.y / parentGraph.NodeCount;
                        }
                        NodeEditorWindow.instance.Repaint();
                    }
                }
            }
        }

        void ProcessContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            if (parentGraph.rootNode == this)
            {
                menu.AddItem(new GUIContent("Un-set as Root"), false, ContextCallback, this);
            }
            else
            {
                menu.AddItem(new GUIContent("Set as Root"), false, ContextCallback, this);
            }
            menu.ShowAsContext();
        }

        void ContextCallback(object obj)
        {
            if (obj is NodeBase)
            {
                NodeBase node = obj as NodeBase;
                if (parentGraph.rootNode == node)
                {
                    parentGraph.rootNode = null;
                    node.input = new NodeInput();
                }
                else
                {
                    if (node.input.parentNode != null)
                    {
                        node.input.parentNode.output.childNodes.Remove(node);
                    }
                    node.parentGraph.rootNode = node;
                    //TODO : call reset here
                }
            }
        }

        void GetEditorSkin()
        {
            if (EditorGUIUtility.isProSkin)
            {
                nodeSkin = Resources.Load("GUI Skins/Editor/NodeEditorDarkSkin") as GUISkin;
            }
            else
            {
                nodeSkin = Resources.Load("GUI Skins/Editor/NodeEditorLightSkin") as GUISkin;
            }
        }
    }
}
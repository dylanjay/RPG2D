using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;

public abstract class NodeBase : ScriptableObject
{
    public string title;
    public string description;

    public Rect nodeRect;
    public NodeGraph parentGraph;
    [HideInInspector]
    public bool isSelected = false;
    [HideInInspector]
    public NodeOutput output;
    [HideInInspector]
    public NodeInput input;
    [HideInInspector]
    public Rect outputRect;
    [HideInInspector]
    public Rect inputRect;
    [HideInInspector]
    public GUISkin nodeSkin;
    Rect viewRect;

    public BehaviorComponent behaviorComponent;


    public static NodeBase CreateNode(System.Type t)
    {
        NodeBase nodeBase = (NodeBase)ScriptableObject.CreateInstance(t);
        nodeBase.title = nodeBase.name = nodeBase.GetType().ToString().Substring(4);
        return nodeBase;
    }

    public NodeBase clone()
    {
        return (NodeBase)this.MemberwiseClone();
    }

    public virtual void Initialize()
    {
        GetEditorSkin();
        if(output != null)
        {
            outputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y + nodeRect.height, 24, 18);
        }

        if(input != null)
        {
            inputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y - 18, 24, 18);
        }
        hideFlags = HideFlags.HideInHierarchy;
    }

    public abstract GUIContent[] GetAllBehaviorOptions();
    public abstract ReadOnlyCollection<System.Type> GetAllBehaviorTypes();

    public virtual void UpdateNode(Event e)
    {
    }
#if UNITY_EDITOR
    public virtual void UpdateNodeGUI(Event e, Rect viewRect)
    {
        this.viewRect = viewRect;
        if(nodeSkin == null)
        {
            GetEditorSkin();
        }
        GUIStyle nodeStyle;
        if(isSelected)
        {
            nodeStyle = nodeSkin.GetStyle("NodeSelected");
        }
        else if(parentGraph.rootNode == this)
        {
            nodeStyle = nodeSkin.GetStyle("NodeRoot");
        }
        else
        {
            nodeStyle = nodeSkin.GetStyle("NodeDefault");
        }
        GUI.Box(nodeRect, name, nodeStyle);

        if(output != null)
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
        if(input == null) { return; }
        if(input.parentNode == null) { return; }

        Vector3 start = input.connectedOutput.position;
        Vector3 end = input.position;
        Vector2 startTangent = new Vector2();
        Vector2 endTangent = new Vector2();
        Color color = Color.gray;
        if(isSelected || input.parentNode.isSelected)
        {
            color = new Color(1, 0.5f, 0);
        }
        float offset = Mathf.Abs(start.x - end.x) / 1.75f;
        if(input.position.x < start.x)
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
        GUILayout.BeginVertical();
        {
            title = EditorGUILayout.TextField("Title", title);
            description = EditorGUILayout.TextField("Description", description);
        }
        GUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();

            int prevOptionNumber = behaviorComponent == null ? 0 : GetAllBehaviorTypes().IndexOf(behaviorComponent.GetType()) + 1;
            int optionNumber = EditorGUILayout.Popup(prevOptionNumber, GetAllBehaviorOptions());

            if (optionNumber != prevOptionNumber)
            {
                bool changeName = (behaviorComponent == null) ? title == name : title == behaviorComponent.name;
                DestroyImmediate(behaviorComponent, true);
                //Option 0 is "None", a null behaviorComponent
                if (optionNumber == 0)
                {
                    behaviorComponent = null;
                    if (changeName)
                    {
                        title = name;
                    }
                }
                else
                {
                    behaviorComponent = BehaviorComponent.CreateComponent(GetAllBehaviorTypes()[optionNumber - 1]);
                    if (changeName)
                    {
                        title = behaviorComponent.name;
                    }
                }
            }

            EditorGUILayout.Space();

            if (behaviorComponent != null)
            {
                EditorGUILayout.LabelField("Parameters");
                foreach (FieldInfo fieldInfo in behaviorComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if ((fieldInfo.GetCustomAttributes(typeof(HideInInspector), true).Length > 0) ||
                       (!fieldInfo.IsPublic && fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Length == 0))
                    {
                        continue;
                    }

                    if (fieldInfo.FieldType == typeof(int))
                    {
                        int value = (int)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.IntField(fieldInfo.Name, value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(float))
                    {
                        float value = (float)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.FloatField(fieldInfo.Name, value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(string))
                    {
                        string value = (string)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.TextField(fieldInfo.Name, value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(Vector2))
                    {
                        Vector2 value = (Vector2)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.Vector2Field(fieldInfo.Name, value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(Vector3))
                    {
                        Vector3 value = (Vector3)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.Vector3Field(fieldInfo.Name, value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    //Note: fieldInfo.FieldType == typeof(UnityEngine.Object) will result in false every time, because
                    //fieldInfo.FieldType will point to a derived class, making the comparison false.
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        UnityEngine.Object value = (UnityEngine.Object)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.ObjectField(fieldInfo.Name, value, fieldInfo.FieldType, false);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
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
                        nodeRect.x += e.delta.x / parentGraph.nodes.Count;
                        nodeRect.y += e.delta.y / parentGraph.nodes.Count;
                    }
                }
            }
        }
    }

    void ProcessContextMenu(Event e)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Set as Root"), false, ContextCallback, this);
        menu.ShowAsContext();
    }

    void ContextCallback(object obj)
    {
        if(obj is NodeBase)
        {
            NodeBase node = obj as NodeBase;
            node.parentGraph.rootNode = node;
            Debug.Log(node.parentGraph.rootNode.title);
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

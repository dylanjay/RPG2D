using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class NodeLeaf : NodeBase
{
    //TODO: Make option number unique, so both renaming and deleting a variable will not cause issues.
    [Serializable]
    protected class ChoiceDictionary : SerializableDictionary<string, GUIContent> { }
    /// <summary>
    /// Key: The name of the field
    /// Value: The option number from the dropdown. Currently deleting a variable in the tree changes the choice.
    /// </summary>
    [SerializeField]
    protected ChoiceDictionary choices = new ChoiceDictionary();

    private static GUIContent[] _allLeafOptions = null;
    private static ReadOnlyCollection<Type> _allLeafTypes = null;

    public override GUIContent[] GetAllBehaviorOptions()
    {
        if (_allLeafOptions == null)
        {
            GUIContent[] tmp = NodeEditorTags.GetAllLabelsOfNodeType(NodeType.Leaf);
            _allLeafOptions = new GUIContent[tmp.Length + 1];
            _allLeafOptions[0] = new GUIContent("None");
            for (int i = 0; i < tmp.Length; i++)
            {
                _allLeafOptions[i + 1] = tmp[i];
            }
        }
        return _allLeafOptions;
    }

    public override ReadOnlyCollection<Type> GetAllBehaviorTypes()
    {
        if (_allLeafTypes == null)
        {
            _allLeafTypes = NodeEditorTags.GetAllTypesOfNodeType(NodeType.Leaf);
        }
        return _allLeafTypes;
    }


    public NodeLeaf()
    {
        input = new NodeInput();
    }

    public override void Reset()
    {
        base.Reset();
        if (output != null)
        {
            output = null;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
        //TODO switch to reset function
        if (output != null)
        {
            output = null;
        }
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
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
            BehaviorComponent tempComponent = EditorGUILayout.ObjectField("Type", behaviorComponent, typeof(BehaviorLeaf), false) as BehaviorComponent;
            int optionNumber = tempComponent == null ? 0 : GetAllBehaviorTypes().IndexOf(tempComponent.GetType()) + 1;

            if (optionNumber != prevOptionNumber)
            {
                bool changeName = (behaviorComponent == null) ? title == name : title == behaviorComponent.name;

                if (prevOptionNumber != 0)
                {
                    parentGraph.RemoveBehavior(this);
                }

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
                parentGraph.AddBehavior(this);

                foreach (FieldInfo fieldInfo in behaviorComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (((((Attribute[])fieldInfo.GetCustomAttributes(typeof(HideInInspector), true)).Length > 0) ||
                        (fieldInfo.IsPrivate && (((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length == 0))))
                    {
                        continue;
                    }
                    if (fieldInfo.FieldType.IsSubClassOfGeneric(typeof(SharedVariable<>)))
                    {
                        choices[fieldInfo.Name] = GUIContent.none;
                    }
                }
            }

            EditorGUILayout.Space();

            if (behaviorComponent != null)
            {
                EditorGUILayout.LabelField("Parameters");
                foreach (FieldInfo fieldInfo in behaviorComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if ((((Attribute[])fieldInfo.GetCustomAttributes(typeof(HideInInspector), true)).Length > 0) ||
                        (fieldInfo.IsPrivate && (((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length == 0)))
                    {
                        continue;
                    }

                    if (fieldInfo.FieldType == typeof(int))
                    {
                        int value = (int)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.IntField(EditorUtilities.FixName(fieldInfo.Name), value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(float))
                    {
                        float value = (float)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.FloatField(EditorUtilities.FixName(fieldInfo.Name), value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(string))
                    {
                        string value = (string)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.TextField(EditorUtilities.FixName(fieldInfo.Name), value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(Vector2))
                    {
                        Vector2 value = (Vector2)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.Vector2Field(EditorUtilities.FixName(fieldInfo.Name), value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType == typeof(Vector3))
                    {
                        Vector3 value = (Vector3)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.Vector3Field(EditorUtilities.FixName(fieldInfo.Name), value);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                    else if (fieldInfo.FieldType.IsSubClassOfGeneric(typeof(SharedVariable<>)))
                    {
                        Type type = (Type)fieldInfo.FieldType.GetProperty("sharedType").GetGetMethod().Invoke(Activator.CreateInstance(fieldInfo.FieldType), new object[] { });
                        GUIContent[] options = parentGraph.GetDropdownOptions(type);
                        int prevChoice = GetGUIIndex(options, choices[fieldInfo.Name]);
                        int currentChoice = EditorGUILayout.Popup(new GUIContent(EditorUtilities.FixName(fieldInfo.Name)), prevChoice, options);
                        if (currentChoice != prevChoice)
                        {
                            choices[fieldInfo.Name] = options[currentChoice];
                            parentGraph.SetReference(this, fieldInfo.Name, options[prevChoice], options[currentChoice]);
                        }
                    }
                    //Note: fieldInfo.FieldType == typeof(UnityEngine.Object) will result in false every time, because
                    //fieldInfo.FieldType will point to a derived class, making the comparison false.
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        UnityEngine.Object value = (UnityEngine.Object)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.ObjectField(EditorUtilities.FixName(fieldInfo.Name), value, fieldInfo.FieldType, false);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                }
            }
        }
    }

    private int GetGUIIndex(GUIContent[] array, GUIContent findMe)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if(array[i] == findMe)
            {
                return i;
            }
        }
        return 0;
    }

    public override void DrawNodeHelp()
    {
        base.DrawNodeHelp();
    }
#endif
}
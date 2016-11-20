using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

public class NodeLeaf : NodeBase
{
    private static GUIContent[] _allLeafOptions = null;
    private static GUIContent[] allLeafOptions
    {
        get
        {
            if(_allLeafOptions == null)
            {
                GUIContent[] tmp = NodeEditorTags.allLeaves;
                _allLeafOptions = new GUIContent[tmp.Length + 1];
                _allLeafOptions[0] = new GUIContent("None");
                for(int i = 0; i < tmp.Length; i++)
                {
                    _allLeafOptions[i + 1] = tmp[i];
                }
            }
            return _allLeafOptions;
        }
    }
    private int optionNumber = 0;

    public NodeLeaf()
    {
        input = new NodeInput();
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    //TODO: Fix saving
    public void SaveBehavior()
    {
        if (behaviorComponent != null)
        {
            string pathToTree = @"Assets/Resources/BehaviorTrees/" + "ConcreteTree" + ".asset";
            AssetDatabase.AddObjectToAsset(behaviorComponent, pathToTree);
            /*for(int i = 1;  i < constructor.GetParameters().Length; i++)
            {
                UnityEngine.Object obj = constructorValues[i] as UnityEngine.Object;
                AssetDatabase.AddObjectToAsset(obj, pathToBehavior);
            }*/
        }
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
        if(output != null)
        {
            output = null;
        }
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
    {
        base.DrawNodeProperties();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();

            int prevOptionNumber = optionNumber;
            optionNumber = EditorGUILayout.Popup(optionNumber, allLeafOptions);
            
            if(optionNumber != prevOptionNumber)
            {
                DestroyImmediate(behaviorComponent, true);
                //Option 0 is "None", a null behaviorComponent
                if(optionNumber == 0)
                {
                    behaviorComponent = null;
                }
                else
                {
                    behaviorComponent = BehaviorComponent.CreateComponent(NodeEditorTags.allLeafTypes[optionNumber - 1]);
                }
            }

            EditorGUILayout.Space();

            if (behaviorComponent != null)
            {
                EditorGUILayout.LabelField("Parameters");
                foreach(FieldInfo fieldInfo in behaviorComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    bool hideInInspector = false;
                    foreach (Attribute attribute in fieldInfo.GetCustomAttributes(false))
                    {
                        if(attribute.GetType() == typeof(HideInInspector))
                        {
                            hideInInspector = true;
                            break;
                        }
                    }
                    if (hideInInspector) { continue; }

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

    public override void DrawNodeHelp()
    {
        base.DrawNodeHelp();
    }
#endif
}

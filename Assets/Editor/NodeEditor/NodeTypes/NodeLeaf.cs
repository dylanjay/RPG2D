using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

public class NodeLeaf : NodeBase
{
    public UnityEngine.Object behavior = null;
    public string pathToBehavior;
    private Type behaviorType;
    private ConstructorInfo constructor;
    private Type[] constructorTypes;
    public object[] constructorValues;

    public NodeLeaf()
    {
        input = new NodeInput();
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public void SetBehavior()
    {
        Debug.Log(pathToBehavior);
        if(pathToBehavior != null || pathToBehavior != string.Empty)
        {
            behavior = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pathToBehavior);
        }
    }

    public void SaveBehavior()
    {
        if (behavior != null)
        {
            pathToBehavior = @"Assets/Editor/NodeEditor/Data/" + behavior.name + ".asset";
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(behavior.GetInstanceID()), pathToBehavior);
            /*for(int i = 1;  i < constructor.GetParameters().Length; i++)
            {
                UnityEngine.Object obj = constructorValues[i] as UnityEngine.Object;
                AssetDatabase.AddObjectToAsset(obj, pathToBehavior);
            }*/
        }
    }

    public override bool CreateTree()
    {
        if (behavior != null)
        {
            behaviorNode = constructor.Invoke(constructorValues) as BehaviorComponent;
            return true;
        }
        return false;
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
            bool behaviorChanged = true;
            UnityEngine.Object behaviorCopy = behavior;
            behavior = EditorGUILayout.ObjectField("Behavior", behavior, typeof(BehaviorLeaf), false);
            if (behavior == behaviorCopy)
            {
                behaviorChanged = false;
            }
            else if(behavior != behaviorCopy && pathToBehavior != string.Empty && pathToBehavior != null)
            {
                AssetDatabase.DeleteAsset(pathToBehavior);
                pathToBehavior = string.Empty;
            }
            EditorGUILayout.Space();
            if (behavior != null)
            {
                EditorGUILayout.LabelField("Parameters");
                behaviorType = behavior.GetType();
                ConstructorInfo[] constructors = behaviorType.GetConstructors();
                constructor = constructors[0];
                constructorTypes = new Type[constructor.GetParameters().Length];
                if (behaviorChanged)
                {
                    constructorValues = new object[constructor.GetParameters().Length];
                    constructorValues[0] = title;
                }
                foreach (ParameterInfo param in constructor.GetParameters())
                {
                    string paramName = param.Name.Substring(0, 1);
                    paramName = paramName.ToUpper();
                    paramName += param.Name.Substring(1);
                    if (param.ParameterType == typeof(int))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        int value = 0;
                        if (!behaviorChanged)
                        {
                            value = (int)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.IntField(paramName, value);
                    }

                    else if (param.ParameterType == typeof(float))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        float value = 0;
                        if (!behaviorChanged)
                        {
                            value = (float)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.FloatField(paramName, value);
                    }

                    else if (param.ParameterType == typeof(string) && param.Position != 0)
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        string value = "";
                        if (!behaviorChanged)
                        {
                            value = (string)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.TextField(paramName, value);
                    }

                    else if (param.ParameterType == typeof(Player))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        Player value = null;
                        if (!behaviorChanged)
                        {
                            value = (Player)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.ObjectField(paramName, value, typeof(Player), true);
                    }

                    else if (param.ParameterType == typeof(Hostile))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        Hostile value = null;
                        if (!behaviorChanged)
                        {
                            value = (Hostile)constructorValues[param.Position];
                        }
                        Debug.Log(param.Position);
                        constructorValues[param.Position] = EditorGUILayout.ObjectField(paramName, value, typeof(Hostile), true);
                    }

                    /*else if(param.ParameterType == typeof(SerializableVector2))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        SerializableVector2 serializedValue = new SerializableVector2();
                        serializedValue.Fill(Vector2.zero);
                        if (!behaviorChanged)
                        {
                            serializedValue = (SerializableVector2)constructorValues[param.Position];
                        }
                        Vector2 value = serializedValue.vector2;
                        serializedValue.Fill(EditorGUILayout.Vector2Field(paramName, value));
                        constructorValues[param.Position] = serializedValue;
                    }

                    else if (param.ParameterType == typeof(SerializableVector3))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        SerializableVector3 serializedValue = new SerializableVector3();
                        serializedValue.Fill(Vector3.zero);
                        if (!behaviorChanged)
                        {
                            serializedValue = (SerializableVector3)constructorValues[param.Position];
                        }
                        Vector2 value = serializedValue.vector3;
                        serializedValue.Fill(EditorGUILayout.Vector3Field(paramName, value));
                        constructorValues[param.Position] = serializedValue;
                    }*/

                    else if (param.ParameterType == typeof(Vector2))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        Vector2 value = Vector2.zero;
                        if (!behaviorChanged)
                        {
                            value = (Vector2)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.Vector2Field(paramName, value);
                    }

                    else if (param.ParameterType == typeof(Vector3))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        Vector3 value = Vector3.zero;
                        if (!behaviorChanged)
                        {
                            value = (Vector3)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.Vector3Field(paramName, value);
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

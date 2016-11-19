using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

/*public class NodeAction : NodeBase
{
    private UnityEngine.Object action;
    private Type actionType;
    private ConstructorInfo constructor;
    private Type[] constructorTypes;
    private object[] constructorValues;

    public NodeAction()
    {
        input = new NodeInput();
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public override bool CreateTree()
    {
        base.CreateTree();
        if(action != null)
        {
            behaviorNode = constructor.Invoke(constructorValues) as BehaviorComponent;
            return true;
        }
        return false;
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
    {
        base.DrawNodeProperties();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();
            bool actionChanged = true;
            UnityEngine.Object actionCopy = action;
            action = EditorGUILayout.ObjectField("Action", action, typeof(BehaviorLeaf), false);
            if (action == actionCopy)
            {
                actionChanged = false;
            }
            EditorGUILayout.Space();
            if (action != null)
            {
                EditorGUILayout.LabelField("Parameters");
                actionType = action.GetType();
                ConstructorInfo[] constructors = actionType.GetConstructors();
                constructor = constructors[0];
                constructorTypes = new Type[constructor.GetParameters().Length];
                if (actionChanged)
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
                        if (!actionChanged)
                        {
                            value = (int)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.IntField(paramName, value);
                    }

                    else if (param.ParameterType == typeof(float))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        float value = 0;
                        if (!actionChanged)
                        {
                            value = (float)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.FloatField(paramName, value);
                    }

                    else if (param.ParameterType == typeof(string) && param.Position != 0)
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        string value = "";
                        if (!actionChanged)
                        {
                            value = (string)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.TextField(paramName, value);
                    }

                    /*else if (param.ParameterType == typeof(Player))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        Player value = null;
                        if (!actionChanged)
                        {
                            value = (Player)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.ObjectField(paramName, value, typeof(Player), true);
                    }

                    else if (param.ParameterType == typeof(Hostile))
                    {
                        constructorTypes[param.Position] = param.ParameterType;
                        Hostile value = null;
                        if (!actionChanged)
                        {
                            value = (Hostile)constructorValues[param.Position];
                        }
                        constructorValues[param.Position] = EditorGUILayout.ObjectField(paramName, value, typeof(Hostile), true);
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
}*/

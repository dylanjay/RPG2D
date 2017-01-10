﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

using Attribute = System.Attribute;
using Type = System.Type;
using Object = UnityEngine.Object;

[CustomEditor(typeof(NodeBase), true)]
public class NodeBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NodeBase node = (NodeBase)target;
        FieldInfo choicesField = node.GetType().GetField("choices", BindingFlags.NonPublic | BindingFlags.Instance);
        BehaviorComponent behaviorComponent = node.behaviorComponent;
        bool behaviorWasEmpty = behaviorComponent == null;
        SerializableDictionary<string, GUIContent> choices = (SerializableDictionary<string, GUIContent>)choicesField.GetValue(node);

        GUILayout.BeginVertical();
        {
            node.title = EditorGUILayout.TextField("Title", node.title);
            node.description = EditorGUILayout.TextField("Description", node.description);
        }

        GUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();

            int prevOptionNumber = behaviorWasEmpty ? 0 : node.GetAllBehaviorTypes().IndexOf(behaviorComponent.GetType()) + 1;
            int optionNumber = EditorGUILayout.Popup(prevOptionNumber, node.GetAllBehaviorOptions());

            if (optionNumber != prevOptionNumber)
            {
                bool changeName = behaviorWasEmpty ? node.title == node.name : node.title == behaviorComponent.name;

                if (prevOptionNumber != 0)
                {
                    node.parentGraph.RemoveBehavior(node);
                }

                DestroyImmediate(behaviorComponent, true);
                //Option 0 is "None", a null behaviorComponent
                if (optionNumber == 0)
                {
                    behaviorComponent = null;
                    if (changeName)
                    {
                        node.title = node.name;
                    }
                }
                else
                {
                    behaviorComponent = BehaviorComponent.CreateComponent(node.GetAllBehaviorTypes()[optionNumber - 1]);
                    if (changeName)
                    {
                        node.title = behaviorComponent.name;
                    }
                    node.behaviorComponent = behaviorComponent;
                    node.parentGraph.AddBehavior(node);
                }

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
            if (!behaviorWasEmpty)
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
                        Type sharedVarType = fieldInfo.FieldType.BaseType.GetGenericArguments()[0];
                        GUIContent[] options = node.parentGraph.GetDropdownOptions(sharedVarType);
                        int prevChoice = GetGUIIndex(options, choices[fieldInfo.Name]);
                        int currentChoice = EditorGUILayout.Popup(new GUIContent(EditorUtilities.FixName(fieldInfo.Name)), prevChoice, options);
                        if (currentChoice != prevChoice)
                        {
                            choices[fieldInfo.Name] = options[currentChoice];
                            node.parentGraph.SetReference(node, fieldInfo.Name, options[prevChoice], options[currentChoice]);
                        }
                    }
                    //Note: fieldInfo.FieldType == typeof(UnityEngine.Object) will result in false every time, because
                    //fieldInfo.FieldType will point to a derived class, making the comparison false.
                    else if (typeof(Object).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        Object value = (Object)fieldInfo.GetValue(behaviorComponent);
                        value = EditorGUILayout.ObjectField(EditorUtilities.FixName(fieldInfo.Name), value, fieldInfo.FieldType, false);
                        fieldInfo.SetValue(behaviorComponent, value);
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();

        choicesField.SetValue(node, choices);
        node.behaviorComponent = behaviorComponent;
    }

    private static int GetGUIIndex(GUIContent[] array, GUIContent findMe)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == findMe)
            {
                return i;
            }
        }
        return 0;
    }
}

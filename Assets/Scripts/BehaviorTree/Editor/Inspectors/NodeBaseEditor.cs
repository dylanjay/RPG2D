using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

using Attribute = System.Attribute;
using Type = System.Type;
using Object = UnityEngine.Object;
using ExtensionMethods;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BehaviorNodeBase), true)]
    public class NodeBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BehaviorNodeBase node = (BehaviorNodeBase)target;
            FieldInfo choicesField = node.GetType().GetField("choices", BindingFlags.NonPublic | 
                                                                        BindingFlags.Instance);
            SerializableDictionary<string, SharedVariable> choices = (SerializableDictionary<string, SharedVariable>)
                                                                     choicesField.GetValue(node);

            BehaviorComponent behaviorComponent = node.behaviorComponent;
            bool behaviorWasEmpty = behaviorComponent == null;

            GUILayout.BeginVertical();
            {
                node.title = EditorGUILayout.TextField("Title", node.title);
                node.description = EditorGUILayout.TextField("Description", node.description);
            }

            GUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space();

                int prevOptionNumber = 0;
                if (!behaviorWasEmpty)
                {
                    prevOptionNumber = 1 + NodeAttributeTags.GetInfo(node.GetType()).FindIndex(
                            x => x.classType == behaviorComponent.GetType()
                    );
                }
                int optionNumber = EditorGUILayout.Popup(prevOptionNumber, 
                                                         NodeAttributeTags.GetNodeOptions(node.GetType()));


                if (optionNumber != prevOptionNumber)
                {
                    bool changeName = behaviorWasEmpty ? node.title == node.name :
                                                         node.title == behaviorComponent.name;

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
                        Type behaviorType = NodeAttributeTags.GetType(node.GetType(), optionNumber - 1);
                        behaviorComponent = BehaviorComponent.CreateComponent(behaviorType);
                        behaviorComponent.hideFlags = HideFlags.HideInHierarchy;
                        AssetDatabase.AddObjectToAsset(behaviorComponent, node.parentGraph);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        if (changeName)
                        {
                            node.title = behaviorComponent.name;
                        }
                        node.behaviorComponent = behaviorComponent;
                        node.parentGraph.AddBehavior(node);
                    }

                    foreach (FieldInfo fieldInfo in behaviorComponent.GetType().GetInstanceFields())
                    {
                        if (fieldInfo.HasAttribute<NonSerializedAttribute>() ||
                            (fieldInfo.IsPrivate && !fieldInfo.HasAttribute<SerializeField>()))
                        {
                            continue;
                        }
                        if (fieldInfo.FieldType.IsSubclassOf(typeof(SharedVariable)))
                        {
                            choices[fieldInfo.Name] = node.parentGraph.sharedVariableCollection.none;
                        }
                    }
                }

                EditorGUILayout.Space();
                if (!behaviorWasEmpty)
                {
                    EditorGUILayout.LabelField("Parameters");
                    foreach (FieldInfo fieldInfo in behaviorComponent.GetType().GetInstanceFields())
                    {
                        if (fieldInfo.HasAttribute<NonSerializedAttribute>() || 
                            fieldInfo.IsPrivate && !fieldInfo.HasAttribute<SerializeField>())
                        {
                            continue;
                        }
                        string fieldName = EditorUtilities.FixName(fieldInfo.Name);
                        if (fieldInfo.FieldType == typeof(int))
                        {
                            int value = (int)fieldInfo.GetValue(behaviorComponent);
                            value = EditorGUILayout.IntField(fieldName, value);
                            fieldInfo.SetValue(behaviorComponent, value);
                        }
                        else if (fieldInfo.FieldType == typeof(float))
                        {
                            float value = (float)fieldInfo.GetValue(behaviorComponent);
                            value = EditorGUILayout.FloatField(fieldName, value);
                            fieldInfo.SetValue(behaviorComponent, value);
                        }
                        else if (fieldInfo.FieldType == typeof(string))
                        {
                            string value = (string)fieldInfo.GetValue(behaviorComponent);
                            value = EditorGUILayout.TextField(fieldName, value);
                            fieldInfo.SetValue(behaviorComponent, value);
                        }
                        else if (fieldInfo.FieldType == typeof(Vector2))
                        {
                            Vector2 value = (Vector2)fieldInfo.GetValue(behaviorComponent);
                            value = EditorGUILayout.Vector2Field(fieldName, value);
                            fieldInfo.SetValue(behaviorComponent, value);
                        }
                        else if (fieldInfo.FieldType == typeof(Vector3))
                        {
                            Vector3 value = (Vector3)fieldInfo.GetValue(behaviorComponent);
                            value = EditorGUILayout.Vector3Field(fieldName, value);
                            fieldInfo.SetValue(behaviorComponent, value);
                        }
                        else if (fieldInfo.FieldType.IsSubclassOf(typeof(SharedVariable)))
                        {
                            Type sharedVarType = fieldInfo.FieldType.BaseType.GetGenericArguments()[0];
                            GUIContent[] options = node.parentGraph.GetDropdownOptions(sharedVarType);
                            int prevChoice = Array.FindIndex(options, x => x.text == choices[fieldInfo.Name].name);
                            int currentChoice = EditorGUILayout.Popup(
                                new GUIContent(EditorUtilities.FixName(fieldInfo.Name)), prevChoice, options
                            );
                            if (currentChoice != prevChoice)
                            {
                                choices[fieldInfo.Name] = node.parentGraph.sharedVariableCollection
                                                              .GetValues()[options[currentChoice].text];
                                node.parentGraph.SetReference(node, 
                                                              fieldInfo.Name, 
                                                              options[prevChoice].text, 
                                                              options[currentChoice].text);
                            }
                        }
                        //Note: fieldInfo.FieldType == typeof(UnityEngine.Object) will result in false every time, 
                        //because fieldInfo.FieldType will point to a derived class, making the comparison false.
                        else if (typeof(Object).IsAssignableFrom(fieldInfo.FieldType))
                        {
                            Object value = (Object)fieldInfo.GetValue(behaviorComponent);
                            value = EditorGUILayout.ObjectField(fieldName, 
                                                                value, 
                                                                fieldInfo.FieldType, 
                                                                allowSceneObjects: false);
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
}

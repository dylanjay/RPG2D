using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

using Attribute = System.Attribute;
using Type = System.Type;
using Object = UnityEngine.Object;

namespace Benco.BehaviorTree.TreeEditor
{
    [CustomEditor(typeof(BehaviorNodeBase), true)]
    public class NodeBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BehaviorNodeBase node = (BehaviorNodeBase)target;
            FieldInfo choicesField = node.GetType().GetField("choices", BindingFlags.NonPublic | BindingFlags.Instance);
            SerializableDictionary<string, SharedVariable> choices = (SerializableDictionary<string, SharedVariable>)choicesField.GetValue(node);

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

                int prevOptionNumber = behaviorWasEmpty ? 0 : 1 + node.GetAllBehaviorTypes().IndexOf(behaviorComponent.GetType());
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

                    foreach (FieldInfo fieldInfo in behaviorComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (((((Attribute[])fieldInfo.GetCustomAttributes(typeof(HideInInspector), true)).Length > 0) ||
                            (fieldInfo.IsPrivate && (((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length == 0))))
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
                        else if (fieldInfo.FieldType.IsSubclassOf(typeof(SharedVariable)))
                        {
                            Type sharedVarType = fieldInfo.FieldType.BaseType.GetGenericArguments()[0];
                            GUIContent[] options = node.parentGraph.GetDropdownOptions(sharedVarType);
                            int prevChoice = Array.FindIndex(options, x => x.text == choices[fieldInfo.Name].name);
                            int currentChoice = EditorGUILayout.Popup(new GUIContent(EditorUtilities.FixName(fieldInfo.Name)), prevChoice, options);
                            if (currentChoice != prevChoice)
                            {
                                choices[fieldInfo.Name] = node.parentGraph.sharedVariableCollection.GetValues()[options[currentChoice].text];
                                node.parentGraph.SetReference(node, fieldInfo.Name, options[prevChoice].text, options[currentChoice].text);
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
}

﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Type = System.Type;

namespace Benco.BehaviorTree.TreeEditor
{
    [CustomEditor(typeof(NodeGraph))]
    public class NodeGraphEditor : Editor
    {
        private int newSharedVariableTypeIndex;
        private string newSharedVariableName = "";

        private GUIStyle _boxGUI;
        private GUIStyle boxGUI
        {
            get
            {
                if (_boxGUI == null)
                {
                    InitializeBoxGUI();
                }
                return _boxGUI;
            }
        }

        private void InitializeBoxGUI()
        {
            _boxGUI = new GUIStyle(GUI.skin.button);
            Texture2D tex2D = new Texture2D(1, 1);
            tex2D.SetPixel(0, 0, new Color(.65f, .65f, .65f));
            tex2D.wrapMode = TextureWrapMode.Repeat;
            tex2D.Apply();
            _boxGUI.normal.background = tex2D;
        }

        public override void OnInspectorGUI()
        {
            NodeGraph graph = (NodeGraph)target;
            GUILayout.BeginVertical();
            {
                graph.graphName = EditorGUILayout.DelayedTextField("Tree Name", graph.graphName);
                graph.description = EditorGUILayout.DelayedTextField("Description", graph.description);
            }

            GUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            {
                EditorGUILayout.Space();

                IDictionary<string, SharedVariable> sharedVariables = graph.sharedVariableCollection.GetValues();
                string removeMe = null;
                string renameString = null;
                string renamedPrevName = null;
                Type renamedVariableType = null;

                Type rmType = null;

                foreach (KeyValuePair<string, SharedVariable> pair in sharedVariables)
                {
                    SharedVariable sharedVariable = pair.Value;
                    if (sharedVariable == null) { continue; }
                    string sharedVariableName = sharedVariable.name;

                    if (sharedVariableName == SharedVariableCollection.emptyChoice) { continue; }

                    Type sharedVarType = sharedVariable.sharedType;

                    EditorGUILayout.BeginVertical(boxGUI);
                    EditorGUILayout.BeginHorizontal();
                    
                    string newName = EditorGUILayout.DelayedTextField(GUIContent.none, sharedVariableName, new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth - 7) });

                    EditorGUILayout.LabelField(sharedVarType.Name, GUILayout.MaxWidth(100));
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(new GUIContent("X", "Remove Variable"), new GUILayoutOption[] { GUILayout.Width(20) }))
                    {
                        removeMe = sharedVariableName;
                        rmType = sharedVarType;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (newName != sharedVariableName)
                    {
                        renameString = newName;
                        renamedPrevName = sharedVariableName;
                        renamedVariableType = sharedVarType;
                    }

                    EditorGUI.indentLevel++;

                    ShowValueEditor(sharedVarType, new GUILayoutOption[] { }, sharedVariable);

                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }

                if (removeMe != null)
                {
                    graph.sharedVariableCollection.RemoveVariable(removeMe, rmType);
                    Repaint();
                }

                if (renameString != null)
                {
                    graph.sharedVariableCollection.RenameVariable(renamedPrevName, renameString, renamedVariableType);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.richText = true;

                EditorGUILayout.LabelField(new GUIContent("<b>Add New Variable</b>"), style);
                newSharedVariableTypeIndex = EditorGUILayout.Popup(newSharedVariableTypeIndex, NodeUtilities.validTypeOptions);
                newSharedVariableName = EditorGUILayout.TextField(newSharedVariableName);
                if (GUILayout.Button(new GUIContent("Add Variable", "Add Variable")))
                {
                    if (newSharedVariableName != "")
                    {
                        graph.sharedVariableCollection.AddVariable(newSharedVariableName, NodeUtilities.validTypes[newSharedVariableTypeIndex]);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void ShowValueEditor(Type type, GUILayoutOption[] options, SharedVariable sharedVariable)
        {
            System.Reflection.FieldInfo valueField = sharedVariable.GetType().GetField("value");
            object value = valueField.GetValue(sharedVariable);
            if (sharedVariable.sharedType == typeof(string) && value == null) { value = ""; }
            System.Func<object, GUILayoutOption[], object> func;
            if (_Fields.TryGetValue(type, out func))
            {
                object newValue = func.Invoke(value, options);

                if (!value.Equals(newValue))
                {
                    valueField.SetValue(sharedVariable, newValue);
                }
            }
            else if (typeof(Object).IsAssignableFrom(type))
            {
                Object newValue = EditorGUILayout.ObjectField(GUIContent.none, (Object)value, type, false);
                valueField.SetValue(sharedVariable, newValue);
            }
            else
            {
                EditorGUILayout.LabelField(type.Name + "s cannot be set here.");
            }
        }

        private static readonly Dictionary<Type, System.Func<object, GUILayoutOption[], object>> _Fields =
            new Dictionary<Type, System.Func<object, GUILayoutOption[], object>>()
            {
                { typeof(int),      (value, options) => {return EditorGUILayout.DelayedIntField((int)value, options); } },
                { typeof(float),    (value, options) => {return EditorGUILayout.DelayedFloatField((float)value, options); } },
                { typeof(double),   (value, options) => {return EditorGUILayout.DelayedDoubleField((float)value, options); } },
                { typeof(string),   (value, options) => {return EditorGUILayout.DelayedTextField((string)value, options); } },
                { typeof(bool),     (value, options) => {return EditorGUILayout.Toggle((bool)value, options); } },
                { typeof(Vector2),  (value, options) => {return EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value, options); } },
                { typeof(Vector3),  (value, options) => {return EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value, options); } },
                { typeof(Vector4),  (value, options) => {return EditorGUILayout.Vector4Field(GUIContent.none, (Vector3)value, options); } },
                { typeof(Bounds),   (value, options) => {return EditorGUILayout.BoundsField((Bounds)value, options); } },
                { typeof(Rect),     (value, options) => {return EditorGUILayout.RectField((Rect)value, options); } },
                { typeof(Object),   (value, options) => {return EditorGUILayout.ObjectField(GUIContent.none, (Object)value, value.GetType(), false, options); } },
            };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Type = System.Type;

[CustomEditor(typeof(NodeGraph))]
public class NodeGraphEditor : Editor
{
    private int newSharedVariableTypeIndex;
    private string newSharedVariableName;

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

            IDictionary<GUIContent, object> sharedVariables = graph.sharedVariableCollection.GetValues();
            GUIContent removeMe = null;
            Type rmType = null;
            
            foreach (GUIContent guiContent in sharedVariables.Keys)
            {
                Debug.Log(guiContent.text);
                if (guiContent.text == "None") { continue; }
                object sharedVariable = sharedVariables[guiContent];
                Type sharedVarType = (Type)sharedVariable.GetType().GetProperty("sharedType").GetGetMethod().Invoke(sharedVariable, new object[0] { });

                Rect rect = EditorGUILayout.BeginHorizontal();

                string name = guiContent.text;
                string newName = EditorGUILayout.DelayedTextField(GUIContent.none, name);

                if (newName != name)
                {
                    guiContent.text = newName;
                }

                ShowValueEditor(sharedVarType, sharedVariable, guiContent);

                if (GUILayout.Button(new GUIContent("X", "Remove Variable")))
                {
                    removeMe = guiContent;
                    rmType = sharedVarType;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeMe != null)
            {
                graph.sharedVariableCollection.RemoveVariable(removeMe.text, rmType);
                Repaint();
            }

            EditorGUILayout.Space();
            newSharedVariableTypeIndex = EditorGUILayout.Popup(newSharedVariableTypeIndex, NodeUtilities.GetValidTypeOptions());
            newSharedVariableName = EditorGUILayout.TextField(newSharedVariableName);
            if (GUILayout.Button(new GUIContent("+", "Add Variable")))
            {
                graph.sharedVariableCollection.AddVariable(newSharedVariableName, NodeUtilities.GetValidTypes()[newSharedVariableTypeIndex]);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private static void ShowValueEditor(Type type, object sharedVariable, GUIContent guiContent)
    {
        object value = sharedVariable.GetType().GetField("value").GetValue(sharedVariable);
        System.Func<object, object> func;
        if (_Fields.TryGetValue(type, out func))
        {
            object newValue = func.Invoke(value);

            if (!value.Equals(newValue))
            {
                sharedVariable.GetType().GetField("value").SetValue(sharedVariable, newValue);
            }
        }
        else if (typeof(Object).IsAssignableFrom(type))
        {
            Object newValue = EditorGUILayout.ObjectField(GUIContent.none, (Object)value, type, false);
            sharedVariable.GetType().GetField("value").SetValue(sharedVariable, newValue);
        }
        else
        {
            EditorGUILayout.LabelField("Type " + type + " is unsupported.");
        }
    }

    private static readonly Dictionary<Type, System.Func<object, object>> _Fields =
        new Dictionary<Type, System.Func<object, object>>()
        {
            { typeof(int),      value => EditorGUILayout.DelayedIntField((int)value)},
            { typeof(float),    value => EditorGUILayout.DelayedFloatField((float)value) },
            { typeof(double),   value => EditorGUILayout.DelayedDoubleField((float)value) },
            { typeof(string),   value => EditorGUILayout.DelayedTextField((string)value) },
            { typeof(bool),     value => EditorGUILayout.Toggle((bool)value) },
            { typeof(Vector2),  value => EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value) },
            { typeof(Vector3),  value => EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value) },
            { typeof(Vector4),  value => EditorGUILayout.Vector4Field(GUIContent.none, (Vector3)value) },
            { typeof(Bounds),   value => EditorGUILayout.BoundsField((Bounds)value) },
            { typeof(Rect),     value => EditorGUILayout.RectField((Rect)value) },
            { typeof(Object),   value => EditorGUILayout.ObjectField(GUIContent.none, (Object)value, value.GetType(), false)},
        };
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NodeBase), true)]
public class NodeBaseDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        //if(Event.current.type == EventType.Layout)
        {
            EditorGUI.BeginProperty(rect, label, property);
            //Debug.Log("Drawing NodeGraph " + property.displayName);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("description"));
            EditorGUI.EndProperty();
        }
    }
}

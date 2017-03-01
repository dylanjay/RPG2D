using UnityEditor;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    [CustomPropertyDrawer(typeof(NodeBase), true)]
    public class NodeBaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            //if(Event.current.type == EventType.Layout)
            {
                EditorGUI.BeginProperty(rect, label, property);
                Debug.Log("Somehow being used.");
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("description"));
                EditorGUI.EndProperty();
            }
        }
    }
}

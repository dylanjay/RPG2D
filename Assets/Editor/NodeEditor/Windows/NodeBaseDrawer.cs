using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NodeGraph), true)]
public class NodeGraphDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //if(Event.current.type == EventType.Layout)
        {
            EditorGUI.BeginProperty(position, label, property);
            Debug.Log("Drawing NodeGraph " + property.displayName + ", " + property.name);
            //EditorGUI.PropertyField(position, property.FindPropertyRelative("NodeBase"));
            EditorGUI.EndProperty();
        }

       /*GUILayout.BeginVertical();
        {
            title = EditorGUILayout.TextField("Title", title);
            description = EditorGUILayout.TextField("Description", description);
        }*/
        /*GUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();
            BehaviorComponent behaviorComponent = (BehaviorComponent)property.FindPropertyRelative("behaviorComponent").objectReferenceValue;
            NodeBase node = (NodeBase)property.objectReferenceValue;

            int prevOptionNumber = behaviorComponent == null ? 0 : node.GetAllBehaviorTypes().IndexOf(behaviorComponent.GetType()) + 1;
            int optionNumber = EditorGUILayout.Popup(prevOptionNumber, node.GetAllBehaviorOptions());

            if (optionNumber != prevOptionNumber)
            {
                bool changeName = (behaviorComponent == null) ? node.title == node.name : node.title == behaviorComponent.name;
                Object.DestroyImmediate(behaviorComponent, true);
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
                }
            }

            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();*/
    }
}
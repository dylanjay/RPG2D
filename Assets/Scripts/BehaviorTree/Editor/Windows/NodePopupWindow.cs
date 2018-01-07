using UnityEngine;
using UnityEditor;
using Benco.BehaviorTree;

namespace Benco.Graph
{
    public class NodePopupWindow : EditorWindow
    {
        string graphName = "Enter a name...";

        public static void Init()
        {
            NodePopupWindow instance = GetWindow<NodePopupWindow>();
            instance.titleContent = new GUIContent("Graph Name");
            instance.maxSize = new Vector2(300, 80);
            instance.minSize = instance.maxSize;
            Rect windowPosition = instance.position;
            windowPosition.center = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
            instance.position = windowPosition;
        }

        public void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                EditorGUILayout.LabelField("New Graph", EditorStyles.boldLabel, GUILayout.Width(80));
                graphName = EditorGUILayout.TextField(graphName);
                GUILayout.Space(20);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                if (GUILayout.Button("Create"))
                {
                    if (!string.IsNullOrEmpty(graphName) && !graphName.Equals("Enter a name..."))
                    {
                        Selection.activeObject = NodeUtilities.CreateNodeGraph<NodeBehaviorTree>(graphName);
                        Close();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Enter a valid name for the graph", "OK");
                    }
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
                GUILayout.Space(20);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
    }
}

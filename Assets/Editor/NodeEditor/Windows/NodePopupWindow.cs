using UnityEngine;
using UnityEditor;

public class NodePopupWindow : EditorWindow
{
    static NodePopupWindow instance;
    string name = "Enter a name...";

    public static void Init()
    {
        instance = GetWindow<NodePopupWindow>();
        instance.titleContent = new GUIContent("Graph Name");
        instance.maxSize = new Vector2(300, 80);
        instance.minSize = instance.maxSize;
    }

    public void OnGUI()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(20);
            EditorGUILayout.LabelField("New Graph", EditorStyles.boldLabel, GUILayout.Width(80));
            name = EditorGUILayout.TextField(name);
            GUILayout.Space(20);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(6);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(20);
            if(GUILayout.Button("Create"))
            {
                if(!string.IsNullOrEmpty(name) && !name.Equals("Enter a name..."))
                {
                    NodeGraph newGraph = NodeUtilities.CreateNodeGraph(name);
                    instance.Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Enter a valid name for the graph", "OK");
                }
            }
            GUILayout.Space(10);
            if(GUILayout.Button("Cancel"))
            {
                instance.Close();
            }
            GUILayout.Space(20);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        Repaint();
    }
}

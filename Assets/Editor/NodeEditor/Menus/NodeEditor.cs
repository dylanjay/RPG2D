using UnityEngine;
using UnityEditor;
using System.Collections;

public class NodeEditor
{
    [MenuItem("Node Editor/Launch Editor")]
    public static void InitNodeEditor()
    {
        NodeEditorWindow.Init();
    }
}

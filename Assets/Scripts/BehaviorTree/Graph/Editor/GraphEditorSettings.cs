using UnityEditor;

public class GraphEditorSettings
{
    public bool snapToGrid { get; set; }
    public bool snapDimensions { get; set; }
    public int snapSize { get; set; }
    public bool showHotbar { get; set; }

    public GraphEditorSettings()
    {
        snapToGrid = EditorPrefs.GetBool("NodeEditor_SnapToGrid", true);
        snapSize = EditorPrefs.GetInt("NodeEditor_SnapSize", 25);
        snapDimensions = EditorPrefs.GetBool("NodeEditor_SnapDimensions", false);
        showHotbar = EditorPrefs.GetBool("NodeEditor_showHotbar", true);
    }

    public void SaveSettings()
    {
        EditorPrefs.SetBool("NodeEditor_SnapToGrid", snapToGrid);
        EditorPrefs.SetInt("NodeEditor_SnapSize", snapSize);
        EditorPrefs.SetBool("NodeEditor_SnapDimensions", snapDimensions);
        EditorPrefs.SetBool("NodeEditor_showHotbar", showHotbar);
    }
}

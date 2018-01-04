using UnityEditor;

namespace Benco.Graph
{
    /// <summary>
    /// A class that maintains the settings for using the GraphEditor.
    /// </summary>
    public class GraphEditorSettings
    {
        public bool snapToGrid { get; set; }
        public bool snapDimensions { get; set; }
        public int snapSize { get; set; }
        public bool showHotbar { get; set; }
        public bool showAxes { get; set; }

        public GraphEditorSettings()
        {
            snapToGrid = EditorPrefs.GetBool("NodeEditor_SnapToGrid", true);
            snapSize = EditorPrefs.GetInt("NodeEditor_SnapSize", 25);
            snapDimensions = EditorPrefs.GetBool("NodeEditor_SnapDimensions", false);
            showHotbar = EditorPrefs.GetBool("NodeEditor_showHotbar", true);
            showAxes = EditorPrefs.GetBool("NodeEditor_showAxes", false);
        }

        public void SaveSettings()
        {
            EditorPrefs.SetBool("NodeEditor_SnapToGrid", snapToGrid);
            EditorPrefs.SetInt("NodeEditor_SnapSize", snapSize);
            EditorPrefs.SetBool("NodeEditor_SnapDimensions", snapDimensions);
            EditorPrefs.SetBool("NodeEditor_showHotbar", showHotbar);
            EditorPrefs.SetBool("NodeEditor_showAxes", showAxes);
        }
    }
}
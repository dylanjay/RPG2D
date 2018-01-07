using System.Text;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class NodeToolbarView : ViewBase
    {
        private static GUILayoutOption buttonHeight;
        private static GUILayoutOption noExpandWidth;

        /// <summary>
        /// The use of this Rect is suggested by: https://docs.unity3d.com/ScriptReference/PopupWindow.html
        /// </summary>
        private Rect dropdownButtonRect;
        private NodeSettingsPopup nodeSettingsPopup;

        public NodeToolbarView(NodeEditorWindow parentWindow) : base(parentWindow)
        {
            if (buttonHeight == null)
            {
                buttonHeight = GUILayout.ExpandHeight(true);
                noExpandWidth = GUILayout.ExpandWidth(false);
            }
            nodeSettingsPopup = new NodeSettingsPopup(parentWindow.graphEditorSettings, parentWindow.graphViewer);
        }

        private GUIStyle GetDropdownStyle()
        {
            return GUI.skin.GetStyle("GV Gizmo DropDown");
        }

        public override void UpdateView(Event e, NodeGraph graph)
        {
            GUI.Box(displayRect, "", EditorStyles.toolbar);
            
            GUILayout.BeginArea(displayRect);
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("New", EditorStyles.toolbarButton, noExpandWidth, buttonHeight))
                    {
                        NodePopupWindow.Init();
                    }
                    if (GUILayout.Button("Load", EditorStyles.toolbarButton, noExpandWidth, buttonHeight))
                    {
                        NodeUtilities.LoadGraph(parentWindow);
                    }
                    GUILayout.Box("", EditorStyles.label, GUILayout.Width(6), buttonHeight);
                    if (graph != null)
                    {
                        if (GUILayout.Button("Save", EditorStyles.toolbarButton, noExpandWidth, buttonHeight))
                        {
                            NodeUtilities.SaveGraph();
                        }

                        if (GUILayout.Button("Clear", EditorStyles.toolbarButton, noExpandWidth, buttonHeight))
                        {
                            NodeUtilities.ClearGraph();
                        }

                        if (GUILayout.Button("Unload", EditorStyles.toolbarButton, noExpandWidth, buttonHeight))
                        {
                            NodeUtilities.UnloadGraph();
                        }

                        if (GUILayout.Button("Delete", EditorStyles.toolbarButton, noExpandWidth, buttonHeight))
                        {
                            NodeUtilities.DeleteGraph();
                        }

                    }
                    GUILayout.FlexibleSpace();
                    GUIContent settingsContent = new GUIContent("Settings", "Show the settings popup content window.");
                    if (GUILayout.Button(settingsContent, GetDropdownStyle()))
                    {
                        dropdownButtonRect.x = displayRect.width + displayRect.x - 206;
                        PopupWindow.Show(dropdownButtonRect, nodeSettingsPopup);
                    }
                    if (e.type == EventType.Repaint)
                    {
                        dropdownButtonRect = GUILayoutUtility.GetLastRect();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }
}

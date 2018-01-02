using System.Text;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class NodeToolbarView : ViewBase
    {
        private static GUILayoutOption buttonHeight;
        private static GUILayoutOption noExpandWidth;
        private Vector2 scrollPosition = Vector2.zero;

        public NodeToolbarView(NodeEditorWindow parentWindow) : base(parentWindow)
        {
            if (buttonHeight == null)
            {
                buttonHeight = GUILayout.ExpandHeight(true);
                noExpandWidth = GUILayout.ExpandWidth(false);
            }
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
                    GUIContent settingsContent = new GUIContent("Settings");
                    if (EditorGUILayout.DropdownButton(settingsContent, FocusType.Passive, GetDropdownStyle()))
                    {
                        Debug.Log("Here");
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            

            EditorGUI.BeginChangeCheck();
            Rect settingsRect = new Rect(displayRect.xMax - 231, displayRect.yMax + 2, 230, 400);
            GUI.Box(settingsRect, "", "WindowBackground");
            GUILayout.BeginArea(settingsRect);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            bool showHotbar = parentWindow.graphEditorSettings.showHotbar;
            parentWindow.graphEditorSettings.showHotbar = EditorGUILayout.Toggle("Show Hot Bar", showHotbar);

            bool snapToGrid = parentWindow.graphEditorSettings.snapToGrid;
            parentWindow.graphEditorSettings.snapToGrid = EditorGUILayout.Toggle("Snap To Grid", snapToGrid);

            EditorGUI.BeginDisabledGroup(!parentWindow.graphEditorSettings.snapToGrid);
            {
                int snapSize = parentWindow.graphEditorSettings.snapSize;
                parentWindow.graphEditorSettings.snapSize = EditorGUILayout.IntField("Snap Size", snapSize);

                bool snapDimensions = parentWindow.graphEditorSettings.snapDimensions;
                parentWindow.graphEditorSettings.snapDimensions = EditorGUILayout.Toggle("Dimensions Match Grid",
                                                                                         snapDimensions);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
            if (EditorGUI.EndChangeCheck())
            {
                parentWindow.graphEditorSettings.SaveSettings();
            }
        }
    }
}

using System.Text;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class NodeToolbarView : ViewBase
    {
        private static GUILayoutOption buttonHeight;
        private static GUILayoutOption noExpandWidth;
        private static GUILayoutOption expandHeight;

        public NodeToolbarView() : base()
        {
            if (buttonHeight == null)
            {
                buttonHeight = GUILayout.ExpandHeight(true);
                noExpandWidth = GUILayout.ExpandWidth(false);
            }
        }

        public override void UpdateView(Rect displayRect, Event e, NodeGraph graph)
        {
            ProcessEvents(e);
            
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
                        NodeUtilities.LoadGraph();
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
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }
}

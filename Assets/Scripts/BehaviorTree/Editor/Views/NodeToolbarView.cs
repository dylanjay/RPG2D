using System.Text;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class NodeToolbarView : ViewBase
    {
        private static GUILayoutOption buttonHeight;
        private static GUILayoutOption noExpandWidth;

        public NodeToolbarView() : base()
        {
            if (buttonHeight == null)
            {
                buttonHeight = GUILayout.Height(viewRect.height);
                noExpandWidth = GUILayout.ExpandWidth(false);
            }
        }

        public void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
        {
            base.UpdateView(editorRect, percentageRect, e);
            ProcessEvents(e);
            
            GUI.Box(viewRect, "", EditorStyles.toolbar);
            GUILayout.BeginArea(viewRect);
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

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
        }

        void ProcessContextMenu(Event e)
        {

        }

        void ContextCallback(object obj)
        {

        }
    }
}

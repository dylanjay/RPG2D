using System.Text;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class NodeToolbarView : ViewBase
    {
        public NodeToolbarView() : base()
        {
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
                    GUILayout.Box("", EditorStyles.label, GUILayout.Height(viewRect.height), GUILayout.Width(6));
                    if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
                    {
                        NodePopupWindow.Init();
                    }

                    GUILayout.Box("", EditorStyles.label, GUILayout.MaxWidth(-3));

                    if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
                    {
                        NodeUtilities.LoadGraph();
                    }
                    GUILayout.Box("", EditorStyles.label, GUILayout.Width(8), GUILayout.Height(viewRect.height));
                    if (graph != null)
                    {
                        if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
                        {
                            NodeUtilities.SaveGraph();
                        }

                        if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
                        {
                            NodeUtilities.ClearGraph();
                        }

                        if (GUILayout.Button("Unload", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
                        {
                            NodeUtilities.UnloadGraph();
                        }

                        if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
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

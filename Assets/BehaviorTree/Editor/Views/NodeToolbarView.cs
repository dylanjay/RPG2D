using System.Text;
using UnityEditor;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public class NodeToolbarView : ViewBase
    {
        public NodeToolbarView() : base("Tools")
        {
        }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeBehaviorTree graph)
        {
            base.UpdateView(editorRect, percentageRect, e, graph);
            ProcessEvents(e);
            
            GUI.Box(viewRect, "", viewSkin.GetStyle("Toolbar"));//.GetStyle("Box"));
            GUILayout.BeginArea(viewRect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Box("", viewSkin.GetStyle("Label"), GUILayout.Height(viewRect.height), GUILayout.Width(6));
                    if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
                    {
                        NodePopupWindow.Init();
                    }

                    GUILayout.Box("", viewSkin.GetStyle("Label"), GUILayout.MaxWidth(-3));

                    if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.Height(viewRect.height)))
                    {
                        NodeUtilities.LoadGraph();
                    }
                    GUILayout.Box("", viewSkin.GetStyle("Label"), GUILayout.Width(8), GUILayout.Height(viewRect.height));
                    if (currentGraph != null)
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
                            NodeUtilities.UnLoadGraph();
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

using UnityEngine;

public class NodeToolbarView : ViewBase
{
    public NodeToolbarView() : base("Tools")
    {

    }

    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
    {
        base.UpdateView(editorRect, percentageRect, e, graph);
        ProcessEvents(e);
        GUI.Box(viewRect, "", viewSkin.GetStyle("ToolsViewBackground"));
        GUILayout.BeginArea(viewRect);
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Height(viewRect.height), GUILayout.Width(2));
                if (GUILayout.Button("New", viewSkin.GetStyle("NewGraphButton"), GUILayout.Width(viewRect.height), GUILayout.Height(viewRect.height)))
                {
                    NodePopupWindow.Init();
                }

                if (GUILayout.Button("Load", viewSkin.GetStyle("LoadGraphButton"), GUILayout.Width(viewRect.height), GUILayout.Height(viewRect.height)))
                {
                    NodeUtilities.LoadGraph();
                }
                GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Height(viewRect.height), GUILayout.Width(8));
                if (currentGraph != null)
                {
                    if (GUILayout.Button("Save", viewSkin.GetStyle("SaveGraphButton"), GUILayout.Width(viewRect.height), GUILayout.Height(viewRect.height)))
                    {
                        NodeUtilities.SaveGraph();
                    }

                    if (GUILayout.Button("Clear", viewSkin.GetStyle("ClearGraphButton"), GUILayout.Width(viewRect.height), GUILayout.Height(viewRect.height)))
                    {
                        NodeUtilities.ClearGraph();
                    }

                    if (GUILayout.Button("Delete", viewSkin.GetStyle("DeleteGraphButton"), GUILayout.Width(viewRect.height), GUILayout.Height(viewRect.height)))
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

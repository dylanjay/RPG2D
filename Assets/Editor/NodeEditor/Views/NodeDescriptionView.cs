using UnityEngine;

public class NodeDescriptionView : ViewBase
{
    public NodeDescriptionView() : base("Description View")
    {

    }

    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
    {
        base.UpdateView(editorRect, percentageRect, e, graph);
        GUI.Box(viewRect, "", viewSkin.GetStyle("DescriptionViewBackground"));
        GUILayout.BeginArea(viewRect);
        {
            if (currentGraph != null)
            {
                if (currentGraph.selectedNode != null)
                {
                    currentGraph.selectedNode.DrawNodeHelp();
                }
            }
        }
        GUILayout.EndArea();
        ProcessEvents(e);
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

using UnityEngine;

public class NodePropertiesView : ViewBase
{
    public NodePropertiesView() : base("Properties View")
    {

    }

    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
    {
        /*base.UpdateView(editorRect, percentageRect, e, graph);
        ProcessEvents(e);
        GUI.Box(viewRect, "", viewSkin.GetStyle("PropertiesViewBackground"));
        GUILayout.BeginArea(viewRect);
        {
            if (currentGraph != null)
            {
                if (currentGraph.showProperties && currentGraph.selectedNode != null)
                {
                    currentGraph.selectedNode.DrawNodeProperties();
                }
                else
                {
                    currentGraph.DrawSharedVariableEditor();
                }
            }
        }
        GUILayout.EndArea();*/
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);
    }
}

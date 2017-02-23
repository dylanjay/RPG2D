using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public class NodeWorkView : ViewBase
    {
        public NodeWorkView() : base("Work View")
        {

        }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
        {
            base.UpdateView(editorRect, percentageRect, e, graph);
            ProcessEvents(e);
            if (currentGraph != null)
            {
                viewTitle = currentGraph.graphName;
                string graphPath = NodeUtilities.currentGraphPath;
                if (graphPath == null || graphPath != @"Assets/Resources/BehaviorTrees/" + currentGraph.graphName + ".asset")
                {
                    NodeUtilities.currentGraphPath = @"Assets/Resources/BehaviorTrees/" + currentGraph.graphName + ".asset";
                }
            }
            else
            {
                viewTitle = "Work View";
            }
            GUI.Box(viewRect, "", viewSkin.GetStyle("WorkViewBackground"));
            GUILayout.BeginArea(viewRect);
            {
                if (currentGraph != null)
                {
                    currentGraph.UpdateGraphGUI(e, viewRect);
                }
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

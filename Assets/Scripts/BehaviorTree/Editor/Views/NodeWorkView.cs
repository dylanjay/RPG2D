using UnityEngine;
using UnityEditor;

namespace Benco.Graph
{
    public class NodeWorkView : ViewBase
    {
        public NodeWorkView() : base("Work View")
        {
        }

        public void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
        {
            base.UpdateView(editorRect, percentageRect, e);
            ProcessEvents(e);
            if (graph != null)
            {
                viewTitle = graph.name;
                string graphPath = NodeUtilities.currentGraphPath;
                string expectedPath = string.Format(@"Assets/Resources/BehaviorTrees/{0}.asset", graph.name);
                if (graphPath == null || graphPath != expectedPath)
                {
                    NodeUtilities.currentGraphPath = expectedPath;
                }
            }
            else
            {
                viewTitle = "Work View";
            }

            GUI.Box(viewRect, "", GUI.skin.GetStyle("flow background"));

            GUILayout.BeginArea(viewRect);
            {
                if (graph != null)
                {
                    NodeEditorWindow.graphController.UpdateGraphGUI(e, viewRect);
                }
            }
            GUILayout.EndArea();
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
        }
    }
}

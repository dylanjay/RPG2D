using UnityEngine;
using UnityEditor;

namespace Benco.Graph
{
    public class NodeWorkView : ViewBase
    {
        public NodeWorkView() : base()
        {
        }

        public override void UpdateView(Rect displayRect, Event e, NodeGraph graph)
        {
            if (graph != null)
            {
                string graphPath = NodeUtilities.currentGraphPath;
                string expectedPath = string.Format(@"Assets/Resources/BehaviorTrees/{0}.asset", graph.name);
                if (graphPath == null || graphPath != expectedPath)
                {
                    NodeUtilities.currentGraphPath = expectedPath;
                }
            }

            GUI.Box(displayRect, "", GUI.skin.GetStyle("flow background"));

            if (graph != null)
            {
                NodeEditorWindow.graphController.UpdateGraphGUI(e, displayRect);
            }
            ProcessEvents(e);
        }

        public override void ProcessEvents(Event e)
        {
            NodeEditorWindow.graphController.OnGUI(e);
        }
    }
}

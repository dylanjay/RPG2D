using UnityEngine;
using UnityEditor;

namespace Benco.Graph
{
    public class NodeWorkView : ViewBase
    {
        public NodeWorkView(NodeEditorWindow parentWindow) : base(parentWindow) { }

        public override void UpdateView(Event e, NodeGraph graph)
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

            if (graph != null)
            {
                //e.mousePosition -= displayRect.position;
                parentWindow.graphViewer.UpdateGraphGUI(e, displayRect);
            }
        }
    }
}

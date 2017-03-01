using UnityEngine;
using UnityEditor;

namespace Benco.BehaviorTree.TreeEditor
{
    public class NodeWorkView : ViewBase
    {
        GUIStyle backgroundStyle;
        public NodeWorkView() : base("Work View")
        {
            backgroundStyle = viewSkin.GetStyle("AnimationEventBackground");
        }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeBehaviorTree graph)
        {

            base.UpdateView(editorRect, percentageRect, e, graph);
            ProcessEvents(e);
            if (currentGraph != null)
            {
                viewTitle = currentGraph.name;
                string graphPath = NodeUtilities.currentGraphPath;
                if (graphPath == null || graphPath != @"Assets/Resources/BehaviorTrees/" + currentGraph.name + ".asset")
                {
                    NodeUtilities.currentGraphPath = @"Assets/Resources/BehaviorTrees/" + currentGraph.name + ".asset";
                }
            }
            else
            {
                viewTitle = "Work View";
            }

            //GUI.DrawTextureWithTexCoords(editorRect, backgroundTexture, new Rect(0, 0, editorRect.width / 120, editorRect.height / 120));//black, new Rect(0, 0, screenBounds.width / backgroundTexture.width, screenBounds.height / backgroundTexture.height));
            GUI.Box(viewRect, "", GUI.skin.GetStyle("TE NodeBackground"));


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

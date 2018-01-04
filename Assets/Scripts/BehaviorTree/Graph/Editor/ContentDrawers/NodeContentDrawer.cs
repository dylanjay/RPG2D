using UnityEngine;

namespace Benco.Graph
{
    public class NodeContentDrawer : ContentDrawer<NodeBase>
    {
        private Rect GetBounds(NodeBase subject, DrawingSettings settings)
        {
            if (settings.snapDimensions && settings.snapSize > 1)
            {
                Rect rect = subject.rect;
                int snapSize = settings.snapSize;
                rect.width = Mathf.Ceil(rect.width / snapSize) * snapSize;
                rect.height = Mathf.Ceil(rect.height / snapSize) * snapSize;
                return rect;
            }
            else
            {
                return new Rect(subject.rect.position, new Vector2(150, 35));
            }
        }

        public override void OnGUI(NodeBase subject, DrawingSettings drawingSettings, Event e)
        {
            if (subject == null) { return; }
            GUIStyle nodeStyle;

            if (subject.isSelected || subject.isHighlighted)
            {
                if (subject.parentGraph.root == subject)
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 3 on");
                }
                else
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 0 on");
                }
            }
            else
            {
                if (subject.parentGraph.root == subject)
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 3");
                }
                else
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 0");
                }
            }
            subject.rect = GetBounds(subject, drawingSettings);
            GUI.Box(subject.rect, subject.title, nodeStyle);
        }
    }
}

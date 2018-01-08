using UnityEngine;
using Benco.Graph;
using UnityEditor;

namespace Benco.BehaviorTree
{
    public class NodeComposite : BehaviorNodeBase
    {
        protected override void Initialize()
        {
            if (input != null && output != null)
            {
                return;
            }

            input = CreateInstance<SingleNodePort>();
            input.Init(this);
            input.hideFlags = HideFlags.HideInHierarchy;

            output = CreateInstance<MultiPort>();
            output.Init(this);
            output.hideFlags = HideFlags.HideInHierarchy;
        }

        public override void OnDraw(DrawingSettings drawingSettings, Event e)
        {
            GUIStyle nodeStyle;

            if (isSelected || isHighlighted)
            {
                if (parentGraph.root == this)
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
                if (parentGraph.root == this)
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 3");
                }
                else
                {
                    nodeStyle = GUI.skin.GetStyle("flow node 0");
                }
            }
            nodeStyle.alignment = TextAnchor.UpperCenter;
            rect = SnapRectToBounds(drawingSettings, 150, 75);
            GUI.Box(rect, title, nodeStyle);

            GUILayout.BeginArea(rect);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Test", GUILayout.Width((rect.width - 10) / 2.0f));
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            EditorGUILayout.DelayedIntField(4);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
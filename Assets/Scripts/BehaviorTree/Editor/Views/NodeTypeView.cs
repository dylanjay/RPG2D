using UnityEngine;
using Benco.BehaviorTree;

namespace Benco.Graph
{
    public class NodeTypeView : ViewBase
    {
        public NodeTypeView() : base()
        {

        }

        public void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
        {
            base.UpdateView(editorRect, percentageRect, e);
            ProcessEvents(e);
            GUI.Box(viewRect, "", GUI.skin.GetStyle("Box"));

            GUIStyle buttonStyle = GUI.skin.GetStyle("Button");
            GUIStyle labelStyle = GUI.skin.GetStyle("Label");

            GUILayout.BeginArea(viewRect);
            {
                GUILayout.BeginVertical();
                {
                    Vector2 spawnLocation = new Vector2(30, 30);
                    GUILayoutOption[] buttonParams = new GUILayoutOption[] { GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2) };
                    if (GUILayout.Button("Seq", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorSequence)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Sel", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorSelector)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("MSeq", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorMemSequence)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("MSel", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), graph, spawnLocation);
                    }

                    GUILayout.Box("", labelStyle, GUILayout.Width(viewRect.width), GUILayout.Height(8));

                    if (GUILayout.Button("Invert", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorInverter)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Succ", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorSucceeder)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Fail", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorFailer)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Run", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Wait", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorWait)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Limiter", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorLimiter)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("MaxTime", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorMaxTime)), graph, spawnLocation);
                    }

                    GUILayout.Box("", labelStyle, GUILayout.Width(viewRect.width), GUILayout.Height(8));

                    if (GUILayout.Button("Leaf", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeLeaf), null, graph, spawnLocation);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
        }
    }
}

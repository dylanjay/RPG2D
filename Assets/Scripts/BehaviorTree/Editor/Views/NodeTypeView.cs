using UnityEngine;
using Benco.BehaviorTree;
using UnityEditor;

namespace Benco.Graph
{
    public class NodeTypeView : ViewBase
    {
        private GUIStyle buttonStyle = GUI.skin.GetStyle("Button");
        private GUIStyle labelStyle = GUI.skin.GetStyle("Label");
        public NodeTypeView() : base()
        {

        }

        public override void UpdateView(Rect displayRect, Event e, NodeGraph graph)
        {
            ProcessEvents(e);

            GUI.Box(displayRect, "", GUI.skin.GetStyle("Box"));

            GUILayout.BeginArea(displayRect);
            {
                GUILayout.BeginVertical();
                {
                    Vector2 spawnLocation = new Vector2(30, 30);
                    GUILayoutOption[] buttonParams = new GUILayoutOption[] {
                        GUILayout.ExpandWidth(true),
                        GUILayout.Height(25)
                    };
                    if (GUILayout.Button("Seq", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorSequence)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Sel", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorSelector)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("MSeq", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorMemSequence)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("MSel", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), graph, spawnLocation);
                    }

                    GUILayout.Box("", labelStyle, GUILayout.Width(displayRect.width), GUILayout.Height(8));

                    if (GUILayout.Button("Invert", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorInverter)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Succ", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorSucceeder)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Fail", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorFailer)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Run", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Wait", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorWait)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Limiter", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorLimiter)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("MaxTime", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorMaxTime)), graph, spawnLocation);
                    }

                    GUILayout.Box("", labelStyle, GUILayout.Width(displayRect.width), GUILayout.Height(8));

                    if (GUILayout.Button("Leaf", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeLeaf), null, graph, spawnLocation);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
    }
}

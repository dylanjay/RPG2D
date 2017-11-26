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

        public override void UpdateView(Event e, NodeGraph graph)
        {
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
                    if (GUILayout.Button("Sequence", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorSequence)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Selector", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorSelector)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Memory Sequence", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorMemSequence)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Memory Selector", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), graph, spawnLocation);
                    }

                    GUILayout.Box("", labelStyle, GUILayout.Width(displayRect.width), GUILayout.Height(8));

                    if (GUILayout.Button("Inverter", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorInverter)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Succeeder", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorSucceeder)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Failer", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorFailer)), graph, spawnLocation);
                    }

                    if (GUILayout.Button("Runner", buttonStyle, buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), 
                            BehaviorComponent.CreateComponent(typeof(BehaviorRunner)), graph, spawnLocation);
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

                    if (GUILayout.Button("Max Time", buttonStyle, buttonParams))
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

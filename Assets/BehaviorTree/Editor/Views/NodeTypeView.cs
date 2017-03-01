using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public class NodeTypeView : ViewBase
    {
        public NodeTypeView() : base("Type View")
        {

        }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeBehaviorTree graph)
        {
            base.UpdateView(editorRect, percentageRect, e, graph);
            ProcessEvents(e);
            GUI.Box(viewRect, "", viewSkin.GetStyle("Box"));
            GUILayout.BeginArea(viewRect);
            {
                GUILayout.BeginVertical();
                {
                    Vector2 spawnLocation = new Vector2(30, 30);
                    GUILayoutOption[] buttonParams = new GUILayoutOption[] { GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2) };
                    if (GUILayout.Button("Seq", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorSequence)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("Sel", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorSelector)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("MSeq", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorMemSequence)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("MSel", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), currentGraph, spawnLocation);
                    }

                    GUILayout.Box("", viewSkin.GetStyle("Label"), GUILayout.Width(viewRect.width), GUILayout.Height(8));

                    if (GUILayout.Button("Invert", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorInverter)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("Succ", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorSucceeder)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("Fail", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorFailer)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("Run", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("Wait", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorWait)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("Limiter", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorLimiter)), currentGraph, spawnLocation);
                    }

                    if (GUILayout.Button("MaxTime", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorMaxTime)), currentGraph, spawnLocation);
                    }

                    GUILayout.Box("", viewSkin.GetStyle("Label"), GUILayout.Width(viewRect.width), GUILayout.Height(8));

                    if (GUILayout.Button("Leaf", viewSkin.GetStyle("Button"), buttonParams))
                    {
                        NodeUtilities.CreateNode(typeof(NodeLeaf), null, currentGraph, spawnLocation);
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

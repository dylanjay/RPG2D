using UnityEngine;

public class NodeTypeView : ViewBase
{
    public NodeTypeView() : base("Type View")
    {

    }

    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph graph)
    {
        base.UpdateView(editorRect, percentageRect, e, graph);
        ProcessEvents(e);
        GUI.Box(viewRect, "", viewSkin.GetStyle("TypeViewBackground"));
        GUILayout.BeginArea(viewRect);
        {
            GUILayout.BeginVertical();
            {
                Vector2 spawnLocation = new Vector2(30, 30);
                GUILayoutOption[] buttonParams = new GUILayoutOption[] { GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)};
                if (GUILayout.Button("Seq", viewSkin.GetStyle("SequenceNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorSequence)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("Sel", viewSkin.GetStyle("SelectorNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorSelector)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("MSeq", viewSkin.GetStyle("MemSequenceNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorMemSequence)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("MSel", viewSkin.GetStyle("MemSelectorNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeComposite), BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), currentGraph, spawnLocation);
                }

                GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Width(viewRect.width), GUILayout.Height(8));

                if (GUILayout.Button("Invert", viewSkin.GetStyle("InverterNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorInverter)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("Succ", viewSkin.GetStyle("SucceederNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorSucceeder)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("Fail", viewSkin.GetStyle("FailerNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorFailer)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("Run", viewSkin.GetStyle("RunnerNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorMemSelector)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("Wait", viewSkin.GetStyle("WaitNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorWait)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("Limiter", viewSkin.GetStyle("LimiterNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorLimiter)), currentGraph, spawnLocation);
                }

                if (GUILayout.Button("MaxTime", viewSkin.GetStyle("MaxTimeNodeButton"), buttonParams))
                {
                    NodeUtilities.CreateNode(typeof(NodeDecorator), BehaviorComponent.CreateComponent(typeof(BehaviorMaxTime)), currentGraph, spawnLocation);
                }   

                GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Width(viewRect.width), GUILayout.Height(8));

                if (GUILayout.Button("Leaf", viewSkin.GetStyle("LeafNodeButton"), buttonParams))
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

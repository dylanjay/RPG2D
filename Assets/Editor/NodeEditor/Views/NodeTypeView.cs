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
                if (GUILayout.Button("Seq", viewSkin.GetStyle("SequenceNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width/2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeComposite), typeof(BehaviorSequence), new Vector2(30, 30));
                }

                if (GUILayout.Button("Sel", viewSkin.GetStyle("SelectorNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeComposite), typeof(BehaviorSelector), new Vector2(30, 30));
                }

                if (GUILayout.Button("MSeq", viewSkin.GetStyle("MemSequenceNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeComposite), typeof(BehaviorMemSequence), new Vector2(30, 30));
                }

                if (GUILayout.Button("MSel", viewSkin.GetStyle("MemSelectorNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeComposite), typeof(BehaviorMemSelector),new Vector2(30, 30));
                }

                GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Width(viewRect.width), GUILayout.Height(8));

                if (GUILayout.Button("Invert", viewSkin.GetStyle("InverterNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeDecorator), typeof(BehaviorInverter), new Vector2(30, 30));
                }

                if (GUILayout.Button("Succ", viewSkin.GetStyle("SucceederNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeDecorator), typeof(BehaviorSucceeder), new Vector2(30, 30));
                }

                if (GUILayout.Button("Fail", viewSkin.GetStyle("FailerNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeDecorator), typeof(BehaviorFailer), new Vector2(30, 30));
                }

                if (GUILayout.Button("Run", viewSkin.GetStyle("RunnerNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeDecorator), typeof(BehaviorMemSelector), new Vector2(30, 30));
                }

                if (GUILayout.Button("Wait", viewSkin.GetStyle("WaitNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeDecorator), typeof(BehaviorWait), new Vector2(30, 30));
                }

                if (GUILayout.Button("Limiter", viewSkin.GetStyle("LimiterNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeDecorator), typeof(BehaviorLimiter), new Vector2(30, 30));
                }

                if (GUILayout.Button("MaxTime", viewSkin.GetStyle("MaxTimeNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width / 2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeDecorator), typeof(BehaviorMaxTime), new Vector2(30, 30));
                }   

                GUILayout.Box("", viewSkin.GetStyle("Separator"), GUILayout.Width(viewRect.width), GUILayout.Height(8));

                if (GUILayout.Button("Leaf", viewSkin.GetStyle("LeafNodeButton"), GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.width/2)))
                {
                    NodeUtilities.CreateNode(currentGraph, typeof(NodeLeaf), null, new Vector2(30, 30));
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

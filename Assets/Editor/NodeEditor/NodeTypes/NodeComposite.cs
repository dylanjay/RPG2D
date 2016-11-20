using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class NodeComposite : NodeBase
{
    public NodeComposite(Type nodeType)
    {
        input = new NodeInput();
        output = new NodeOutput();
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
    {
        base.DrawNodeProperties();
    }

    public override void DrawNodeHelp()
    {
        base.DrawNodeHelp();
    }
#endif
}

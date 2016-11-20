using UnityEngine;
using System.Linq;

public class NodeRoot : NodeBase
{
    public NodeRoot()
    {
        output = new NodeOutput(); 
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public override void UpdateNode(Event e)
    {
        base.UpdateNode(e);
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
        if(input != null)
        {
            input = null;
        }
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
    {
        base.DrawNodeProperties();
    }
#endif
}

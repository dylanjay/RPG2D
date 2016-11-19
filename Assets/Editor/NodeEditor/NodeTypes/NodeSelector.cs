using UnityEngine;
using System.Linq;

[System.Serializable]
public class NodeSelector : NodeBase
{
    public NodeSelector()
    {
        input = new NodeInput();
        output = new NodeOutput();
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public override bool CreateTree()
    {
        if (output != null)
        {
            if (output.childNodes.Any())
            {
                BehaviorComponent[] childBehaviors = new BehaviorComponent[output.childNodes.Count];
                for (int i = 0; i < output.childNodes.Count; i++)
                {
                    childBehaviors[i] = output.childNodes[i].behaviorNode;
                }
                behaviorNode = new BehaviorSelector(title, childBehaviors);
                return true;
            }
        }
        return false;
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

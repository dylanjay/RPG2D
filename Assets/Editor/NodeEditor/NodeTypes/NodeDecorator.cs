using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class NodeDecorator : NodeBase
{
    private Type nodeType;
    private object[] args = new object[2];

    public NodeDecorator(Type nodeType)
    {
        input = new NodeInput();
        output = new NodeOutput();
        this.nodeType = nodeType;
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
                args[0] = title;
                args[1] = childBehaviors;
                behaviorNode = Activator.CreateInstance(nodeType, args) as BehaviorComponent;
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

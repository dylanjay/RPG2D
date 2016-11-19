using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BehaviorComposite : BehaviorComponent
{
    public BehaviorComponent[] childBehaviors;
	
    public BehaviorComposite(string name) : base(name)
    {

    }

    public BehaviorComposite(string name, BehaviorComponent[] childBehaviors) : base(name)
    {
        this.childBehaviors = childBehaviors;
    }

    public void Shuffle()
    {
        for (int i = 0; i < childBehaviors.Length; i++)
        {
            BehaviorComponent temp = childBehaviors[i];
            int rand = UnityEngine.Random.Range(i, childBehaviors.Length);
            childBehaviors[i] = childBehaviors[rand];
            childBehaviors[rand] = temp;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorDecorator : BehaviorComponent
{
    [SerializeField]
    protected BehaviorComponent childBehavior;
    
    protected override void Instantiate(string name)
    {
        Initialize(name, null);
    }

    public virtual void Initialize(string name, BehaviorComponent childBehavior = null)
    {
        base.Instantiate(name);
        this.childBehavior = childBehavior;
    }

    public override IEnumerator<BehaviorComponent> GetChildren()
    {
        yield return childBehavior;
    }
}

using UnityEngine;

public abstract class BehaviorDecorator : BehaviorComponent {

    [HideInInspector]
    public BehaviorComponent childBehavior;
    
    protected override void Initialize(string name)
    {
        Initialize(name, null);
    }

    protected virtual void Initialize(string name, BehaviorComponent childBehavior = null)
    {
        base.Initialize(name);
        this.childBehavior = childBehavior;
    }

}

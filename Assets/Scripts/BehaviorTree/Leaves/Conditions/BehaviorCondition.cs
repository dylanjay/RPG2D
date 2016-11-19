using UnityEngine;
using System;

/*public class BehaviorCondition : BehaviorLeaf
{
    [SerializeField]
    protected Func<BehaviorState> func;

    public BehaviorCondition(string name, Func<BehaviorState> func) : base(name)
    {
        this.func = func;
    }
    public BehaviorCondition(string name) : base(name)
    {

    }

    public override BehaviorState Behave()
    {
        _returnState = func();
        return _returnState;
    }

    public abstract BehaviorState Update();
}

public class BehaviorCondition<T> : BehaviorLeaf
{
    [SerializeField]
    protected Func<T, BehaviorState> func;

    T value;

    public BehaviorCondition(string name, Func<T, BehaviorState> func, T value) : base(name)
    {
        this.func = func;
        this.value = value;
    }

    public BehaviorCondition(string name) : base(name)
    {

    }

    public override BehaviorState Behave()
    {
        _returnState = func(value);
        return _returnState;
    }
}*/

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// The default class for a leaf node of a behavior tree.
/// 
/// This class is not abstract because it can be used as 
/// a wrapper around a behavior.
/// </summary>
public class BehaviorLeaf : BehaviorComponent {

    [SerializeField]
    protected Func<BehaviorState> func;
    
    public BehaviorLeaf(string name, Func<BehaviorState> func) : base(name)
    {
        func = this.func;
    }
    public BehaviorLeaf(string name) : base(name)
    {
    }

    public override BehaviorState Behave()
    {
        //_returnState = func.Invoke(func);
        return _returnState;
    }
}

public class BehaviorLeaf<T> : BehaviorComponent
{

    [SerializeField]
    protected Func<T, BehaviorState> func;

    T value;
    
    public BehaviorLeaf(string name, Func<T, BehaviorState> func, T value) : base(name)
    {
        this.func = func;
        this.value = value;
    }

    public BehaviorLeaf(string name) : base(name)
    {

    }

    public override BehaviorState Behave()
    {
        //_returnState = func.Invoke(func);
        return _returnState;
    }
}

using UnityEngine;
using System;
/// <summary>
/// The default class for a leaf node of a behavior tree.
/// 
/// This class is not abstract because it can be used as 
/// a wrapper around a behavior.
/// </summary>
/*public class ActionFunc : BehaviorLeaf
{
    [SerializeField]
    protected Func<BehaviorState> func;

    public ActionFunc(string name, Func<BehaviorState> func) : base(name)
    {
        this.func = func;
    }
    public ActionFunc(string name) : base(name)
    {

    }

    public override BehaviorState Behave()
    {
        //Debug.Log(name);
        _returnState = func();
        return _returnState;
    }
}

public class ActionFunc<T> : BehaviorLeaf
{
    [SerializeField]
    protected Func<T, BehaviorState> func;

    T value;

    public ActionFunc(string name, Func<T, BehaviorState> func, T value) : base(name)
    {
        this.func = func;
        this.value = value;
    }

    public ActionFunc(string name) : base(name)
    {

    }

    public override BehaviorState Behave()
    {
        //Debug.Log(name);
        _returnState = func(value);
        return _returnState;
    }
}*/

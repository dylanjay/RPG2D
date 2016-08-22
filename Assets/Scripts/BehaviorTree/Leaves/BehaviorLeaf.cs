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
    protected Behavior behavior;
    protected Func<object, Transform> func;

    public void Dank()
    {
        func = delegate (object s) { return GameObject.Find("").GetComponent<Transform>().GetChild((int)s); };
    }

    public BehaviorLeaf(Behavior behavior)
    {
        behavior = this.behavior;
    }

    public override BehaviorState Behave()
    {
        _returnState = behavior();
        return _returnState;
    }
}

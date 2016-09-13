using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting repeater. Behaves like a conditional while true statement:
/// 
/// Reprocesses child behavior when the child returns a value
/// </summary>

public class BehaviorRepeater : BehaviorDecorator
{
    public BehaviorRepeater(string name, BehaviorComponent childBehavior) : base(name, childBehavior)
    {

    }

    public override BehaviorState Behave()
    {
        returnState = _Behave();
        return returnState;
    }

    /// <summary>
    /// A helper function for the meat of the behavior
    /// </summary>
    /// <returns></returns>
    private BehaviorState _Behave()
    {
        BehaviorState childState = childBehavior.Behave();
        Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");

        switch(childState)
        {
            case BehaviorState.Error:
                return childState;

            default:
                return BehaviorState.Running;
        }
    }
}

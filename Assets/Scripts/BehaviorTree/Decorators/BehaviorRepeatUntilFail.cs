using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting repeater. Behaves like a conditional while statement:
/// 
/// Reprocesses child behavior while the child returns Success
/// </summary>

public class BehaviorRepeatUntilFail : BehaviorDecorator
{
    public BehaviorRepeatUntilFail(string name, BehaviorComponent childBehavior) : base(name, childBehavior)
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

        if (childState == BehaviorState.Success)
        {
            return BehaviorState.Running;
        }

        return BehaviorState.Failure;

        //return BehaviorState.Success;
    }
}

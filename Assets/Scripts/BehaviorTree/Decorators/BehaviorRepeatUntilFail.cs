using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting repeater. Behaves like a conditional while statement:
/// 
/// Reprocesses child behavior until the child returns failure and subsequently returns success
/// </summary>

public class BehaviorRepeatUntilFail : BehaviorDecorator
{
    bool failure = false;

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
        if (failure)
        {
            return BehaviorState.Success;
        }

        BehaviorState childState = childBehavior.Behave();
        Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");

        switch(childState)
        {
            case BehaviorState.Failure:
                failure = true;
                return BehaviorState.Success;

            case BehaviorState.Success:
                return BehaviorState.Running;

            default:
                return childState;
        }
    }
}

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting succeeder. Behaves like a conditional TRUE statement:
/// 
/// Always returns Success regardless of child's return value
/// </summary>

public class BehaviorSucceeder : BehaviorDecorator
{
    public BehaviorSucceeder(string name, BehaviorComponent childBehavior) : base(name, childBehavior)
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

        if(childState == BehaviorState.Error)
        {
            return childState;
        }

        return BehaviorState.Success;
    }
}

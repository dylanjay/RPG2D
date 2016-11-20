using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting failer. Behaves like a conditional FALSE statement:
/// 
/// Always returns Failure regardless of child's return value
/// </summary>

public class BehaviorFailer : BehaviorDecorator
{
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

        return BehaviorState.Failure;
    }
}

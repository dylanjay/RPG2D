using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This decorator imposes a maximum number of calls its child can have within the whole execution of the Behavior Tree
/// 
/// Executes child on n ticks of Update
/// Subsequently returns Failure
/// </summary>

public class BehaviorLimiter : BehaviorDecorator
{
    int iteration = 0;
    int maxIterations = 1;

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
        if(iteration >= maxIterations)
        {
            return BehaviorState.Failure;
        }

        BehaviorState childState = childBehavior.Behave();
        Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");

        return childState;
    }
}

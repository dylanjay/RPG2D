using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting repeater. Behaves like a for statement:
/// 
/// Reprocesses child behavior n times, or it returns error, and subsequently always returns success
/// </summary>

public class BehaviorRepeater : BehaviorDecorator
{
    int maxRepetitions = 1;
    int repetition = 0;
    
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
        if(repetition >= maxRepetitions)
        {
            return BehaviorState.Success;
        }

        BehaviorState childState = childBehavior.Behave();
        Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");

        switch (childState)
        {
            case BehaviorState.Error:
                return childState;

            case BehaviorState.Running:
                return childState;
            
            default:
                repetition++;
                return BehaviorState.Running;
        }
    }
}

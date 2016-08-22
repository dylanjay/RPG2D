using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting sequencer. Behaves like a conditional statement:
/// 
/// Returns Failure/Error/Running on the first failure/error/running. Will not run any behaviors after that.
/// Returns Success if and only if each child has returned success.
/// </summary>

public class BTSequence : BehaviorComposite
{
    /// <summary>
    /// This is a summary.
    /// </summary>
    int currentChild = 0;

    /// <summary>
    /// A helper function to reset the behavior
    /// </summary>
    private void Reset()
    {
        currentChild = 0;
    }

    public override BehaviorState Behave()
    {
        if(returnState != BehaviorState.Running)
        {
            Reset();
        }
        returnState = _Behave();
        return returnState;
    }

    /// <summary>
    /// A helper function for the meat of the behavior
    /// </summary>
    /// <returns></returns>
    private BehaviorState _Behave()
    {
        Debug.Assert(childBehaviors.Count > 0, "Error: Behavior \"" + name + "\" has no children.");
        while (currentChild < childBehaviors.Count)
        {
            BehaviorState childState = childBehaviors[currentChild].Behave();
            Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehaviors[currentChild].name + "\" of behavior \"" + name + "\" has no defined behavior.");

            if (childState != BehaviorState.Success)
            {
                returnState = childState;
                return childState;
            }
            else
            {
                currentChild++;
            }
        }
        currentChild = 0;
        return BehaviorState.Success;
    }
}

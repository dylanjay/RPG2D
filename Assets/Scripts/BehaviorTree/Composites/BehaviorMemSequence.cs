using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting sequencer. Behaves like a conditional AND statement:
/// 
/// MemSequence is similar to Sequence node, but when a child returns a RUNNING state
/// its index is recorded and in the next tick the MemPriority call the child recorded directly
/// without calling previous children again.
/// 
/// Returns Failure/Error on the first failure/error. Will not run any behaviors after that.
/// Returns Success if and only if each child has returned success.
/// </summary>

[ShowInNodeEditor("Memory Sequence", true)]
public class BehaviorMemSequence : BehaviorComposite
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
        if (returnState == BehaviorState.Failure || returnState == BehaviorState.Error)
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
        while (currentChild < childBehaviors.Length)
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

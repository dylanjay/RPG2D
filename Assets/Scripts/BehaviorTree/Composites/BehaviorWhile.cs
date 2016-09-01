using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting sequencer. Behaves like a conditional statement:
/// 
/// Returns Failure/Error/Running on the first failure/error/running. Will not run any behaviors after that.
/// Returns Success if and only if each child has returned success.
/// </summary>

public class BehaviorWhile : BehaviorComposite
{
    private const int CONDTION_BEHAVIOR = 0;
    private const int ACTION_BEHAVIOR = 0;

    private BehaviorComponent condition { get { return childBehaviors[CONDTION_BEHAVIOR]; } }
    private BehaviorComponent action { get { return childBehaviors[ACTION_BEHAVIOR]; } }

    public BehaviorWhile(string name, BehaviorComponent[] childBehaviors) : base(name, childBehaviors)
    {
        Debug.Assert(childBehaviors.Length == 2, "Error: BehaviorWhile's can only have two children. The first is a condition, the second is the action.");
        this.childBehaviors = childBehaviors;
    }

    public BehaviorWhile(string name, BehaviorComponent condition, BehaviorComponent action) : base(name, new BehaviorComponent[2] { condition, action })
    {

    }

    public override BehaviorState Behave()
    {
        returnState = _Behave();
        return returnState;
    }

    /// <summary>
    /// A helper function for the meat of the behavior.
    /// </summary>
    /// <returns></returns>
    private BehaviorState _Behave()
    {
        if (condition.Behave() == BehaviorState.Failure)
        {
            //We return success because the while loop has completed.
            return BehaviorState.Success;
        }
        else
        {
            BehaviorState temp = action.Behave();
            if (temp != BehaviorState.Error)
            {
                return BehaviorState.Running;
            }
            else
            {
                return BehaviorState.Error;
            }
        }
    }
}

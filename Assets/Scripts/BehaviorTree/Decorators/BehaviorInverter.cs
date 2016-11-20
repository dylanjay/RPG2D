using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting inverter. Behaves like a conditional NOT statement:
/// 
/// Returns Failure if child returns Success.
/// Returns Success if child return Failure.
/// Returns Running if child returns Running.
/// </summary>

[ShowInNodeEditor("Inverter", true)]
public class BehaviorInverter : BehaviorDecorator
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

        switch(childState)
        {
            case BehaviorState.Success:
                return BehaviorState.Failure;

            case BehaviorState.Failure:
                return BehaviorState.Success;

            default:
                return childState;
        }
    }
}

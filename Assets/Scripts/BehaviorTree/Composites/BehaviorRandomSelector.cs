﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A short circuiting selector. Behaves like a conditional OR statement:
/// 
/// Iterates through children and returns success on first success
/// Returns failure if and only if all children have returned failure
/// </summary>

public class BehaviorRandomSelector : BehaviorComposite
{
    /// <summary>
    /// This is a summary.
    /// </summary>
    int currentChild = 0;

    public BehaviorRandomSelector(string name, BehaviorComponent[] childBehaviors) : base(name, childBehaviors)
    {
        Shuffle();
    }

    /// <summary>
    /// A helper function to reset the behavior
    /// </summary>
    private void Reset()
    {
        currentChild = 0;
        Shuffle();
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
        Debug.Assert(childBehaviors.Length > 0, "Error: Behavior \"" + name + "\" has no children.");
        while (currentChild < childBehaviors.Length)
        {
            BehaviorState childState = childBehaviors[currentChild].Behave();
            Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehaviors[currentChild].name + "\" of behavior \"" + name + "\" has no defined behavior.");

            if (childState == BehaviorState.Success)
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
        Shuffle();
        return BehaviorState.Failure;
    }
}

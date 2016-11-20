﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Max Time limits the maximum time its child can be running
/// If the child does not complete its execution before the maximum time, the child task is terminated and a failure is returned
/// </summary>
[ShowInNodeEditor("Max Time", true)]
public class BehaviorMaxTime : BehaviorDecorator
{
    float time = 0.0f;
    [SerializeField]
    float maxTime = 1.0f;

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
        if(time >= maxTime)
        {
            return BehaviorState.Failure;
        }

        BehaviorState childState = childBehavior.Behave();
        Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");

        time += Time.deltaTime;
        return childState;
    }
}

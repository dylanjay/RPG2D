using UnityEngine;
using System.Collections;
using System;

public class ActionWait : Action
{
    float time = 0.0f;
    float maxTime;
    Action action;

    public ActionWait(string name, float maxTime, Action action) : base(name)
    {
        this.maxTime = maxTime;
        this.action = action;
    }

    void Reset()
    {
        time = 0.0f;
    }

    public override BehaviorState Behave()
    {
        returnState = _Behave();
        return returnState;
    }

    private BehaviorState _Behave()
    {
        if(time >= maxTime)
        {
            BehaviorState actionState = action.Behave();
            Debug.Assert(actionState != BehaviorState.None, "Error: Child behavior \"" + action.name + "\" of behavior \"" + name + "\" has no defined behavior.");
            if (actionState == BehaviorState.Success)
            {
                Reset();
            }
            return actionState;
            //return BehaviorState.Success?
        }

        time += Time.deltaTime;

        return BehaviorState.Running;
    }
}

using UnityEngine;
using System.Collections;
using System;

public class ActionWait : Action
{
    float time = 0.0f;
    float maxTime;
    Action action;
    Hostile hostile;

    public ActionWait(string name, float maxTime, Hostile hostile, Action action) : base(name)
    {
        this.maxTime = maxTime;
        this.action = action;
        this.hostile = hostile;
    }

    void Reset()
    {
        time = 0.0f;
    }

    public override void Start()
    {
        hostile.anim.SetTrigger(Hostile.AnimParams.Idle);
    }

    public override BehaviorState Update()
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

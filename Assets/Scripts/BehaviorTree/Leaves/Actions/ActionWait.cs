using UnityEngine;
using System.Collections;
using System;

public class ActionWait : Action
{
    float time = 0.0f;
    float maxTime;
    Hostile hostile;

    public ActionWait(string name, float maxTime, Hostile hostile) : base(name)
    {
        this.maxTime = maxTime;
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
            Reset();
            return BehaviorState.Success;
        }

        time += Time.deltaTime;

        return BehaviorState.Running;
    }
}

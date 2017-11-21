using UnityEngine;
using System.Collections.Generic;
using System;
using Benco.BehaviorTree;
using Benco.Graph;

[ShowInNodeEditor("Wait For Seconds")]
public class ActionWait : BehaviorLeaf
{
    float time = 0.0f;
    [SerializeField]
    float maxTime = 1.0f;
    
    public override void OnStart()
    {
        entity.anim.SetTrigger(Hostile.AnimParams.Idle);
        time = 0.0f;
    }

    public override BehaviorState Update()
    {
        time += Time.deltaTime;
        if (time >= maxTime)
        {
            return BehaviorState.Success;
        }

        return BehaviorState.Running;
    }
}

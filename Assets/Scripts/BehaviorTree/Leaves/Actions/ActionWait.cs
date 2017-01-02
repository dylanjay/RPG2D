using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Wait", menuName = "Actions/Wait", order = 0)]
[ShowInNodeEditor("Wait For Seconds", false)]
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

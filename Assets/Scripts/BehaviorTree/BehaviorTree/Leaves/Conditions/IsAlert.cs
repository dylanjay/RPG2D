using UnityEngine;
using System.Collections.Generic;
using System;
using Benco.Graph;
using Benco.BehaviorTree;

[ShowInNodeEditor("Is Alert", "{0}/Conditions/Is Alert")]
public class IsAlert : BehaviorLeaf
{
    SharedTransform target;
    [SerializeField]
    float alertRadius = 1.0f;

    public override void OnStart()
    {
        
    }

    public override BehaviorState Update()
    {
        if (Vector2.Distance(transform.position, target.value.position) <= alertRadius)
        {
            return BehaviorState.Success;
        }
        else
        {
            return BehaviorState.Failure;
        }
    }
}

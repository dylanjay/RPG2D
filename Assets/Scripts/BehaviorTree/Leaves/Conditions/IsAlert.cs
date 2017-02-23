using UnityEngine;
using System.Collections.Generic;
using System;
using Benco.BehaviorTree;

[CreateAssetMenu(fileName = "New Condition Is Alert", menuName = "Conditions/IsAlert", order = 0)]
[ShowInNodeEditor("IsAlert", false)]
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

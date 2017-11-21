using UnityEngine;
using System.Collections.Generic;
using System;
using Benco.Graph;
using Benco.BehaviorTree;

[ShowInNodeEditor("In Attack Range")]
public class InAttackRange : BehaviorLeaf
{
    SharedTransform target;

    [SerializeField]
    float attackRadius = 1.0f;

    public override void OnStart()
    {

    }

    public override BehaviorState Update()
    {
        if (Vector2.Distance(transform.position, target.value.position) <= attackRadius)
        {
            return BehaviorState.Success;
        }

        else
        {
            return BehaviorState.Failure;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System;
using Benco.BehaviorTree;

[CreateAssetMenu(fileName = "New Condition In Attack Range", menuName = "Conditions/InAttackRange", order = 1)]
[ShowInNodeEditor("InAttackRange", false)]
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

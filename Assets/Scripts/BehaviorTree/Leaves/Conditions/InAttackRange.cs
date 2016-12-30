using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Condition In Attack Range", menuName = "Conditions/InAttackRange", order = 1)]
[ShowInNodeEditor("InAttackRange", false)]
public class InAttackRange : BehaviorLeaf
{
    SharedTransform target;
    SharedHostile hostile;
    [SerializeField]
    float attackRadius = 1.0f;

    public override void Init(Dictionary<string, object> sharedVarDict)
    {
        target.Value = (Transform)sharedVarDict[target.name];
        hostile.Value = ((GameObject)sharedVarDict[hostile.name]).GetComponent<Hostile>();
    }

    public override void Start()
    {
        
    }

    public override BehaviorState Update()
    {
        if (Vector2.Distance(hostile.Value.transform.position, target.Value.transform.position) <= attackRadius)
        {
            return BehaviorState.Success;
        }

        else
        {
            return BehaviorState.Failure;
        }
    }
}

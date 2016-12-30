using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Condition Is Alert", menuName = "Conditions/IsAlert", order = 0)]
[ShowInNodeEditor("IsAlert", false)]
public class IsAlert : BehaviorLeaf
{
    SharedTransform target;
    SharedHostile hostile;
    [SerializeField]
    float alertRadius = 1.0f;

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
        if (Vector2.Distance(hostile.Value.transform.position, target.Value.transform.position) <= alertRadius)
        {
            return BehaviorState.Success;
        }

        else
        {
            return BehaviorState.Failure;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Pounce", menuName = "Actions/Pounce", order = 3)]
[ShowInNodeEditor("Pounce", false)]
public class ActionPounce : BehaviorLeaf
{
    [SerializeField]
    private SharedTransform target;
    [SerializeField]
    private float force = 3.0f;
    private Vector2 startPos;
    
    public override void OnStart()
    {
        entity.anim.SetTrigger(Hostile.AnimParams.Pounce);
        startPos = transform.position;
    }

    public override BehaviorState Update()
    {
        Vector2 curPos = transform.position;

        transform.position = Vector2.MoveTowards(curPos, target.value.transform.position, entity.moveSpeed.value * force * Time.deltaTime);

        if(entity.hitPlayer)
        {
            entity.hitPlayer = false;
            entity.transform.position = startPos;
            return BehaviorState.Success;
        }

        return BehaviorState.Running;
    }
}

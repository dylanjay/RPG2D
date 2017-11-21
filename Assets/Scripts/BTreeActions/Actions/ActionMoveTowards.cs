using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benco.BehaviorTree;
using Benco.Graph;

[CreateAssetMenu(fileName = "New Action Move Towards", menuName = "Actions/MoveTowards", order = 2)]
[ShowInNodeEditor("Move Towards")]
public class ActionMoveTowards : BehaviorLeaf
{
    [SerializeField]
    private SharedTransform target;

    public override void OnStart()
    {
        entity.anim.SetTrigger(Hostile.AnimParams.Alert);
    }

    public override BehaviorState Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.value.position, entity.moveSpeed.value * Time.deltaTime);

        return BehaviorState.Running;
    }
}
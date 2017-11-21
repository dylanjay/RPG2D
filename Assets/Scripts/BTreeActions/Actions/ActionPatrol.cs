using UnityEngine;
using System.Collections.Generic;
using System;
using Benco.BehaviorTree;
using Benco.Graph;

[CreateAssetMenu(fileName = "New Action Patrol", menuName = "Actions/Patrol", order = 1)]
[ShowInNodeEditor("Patrol")]
public class ActionPatrol : BehaviorLeaf
{
    [SerializeField]
    private Vector2 waypoint1;
    [SerializeField]
    private Vector2 waypoint2;
    private bool towardsFirstWaypoint = true;
    
    public override void OnStart()
    {
        entity.anim.SetTrigger(Hostile.AnimParams.Patrol);
    }

    public override BehaviorState Update()
    {
        Vector2 curPos = entity.transform.position;
        if (towardsFirstWaypoint)
        {
            transform.position = Vector2.MoveTowards(curPos, waypoint1, entity.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint1)
            {
                towardsFirstWaypoint = false;
            }
        }

        else
        {
            transform.position = Vector2.MoveTowards(curPos, waypoint2, entity.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint2)
            {
                towardsFirstWaypoint = true;
            }
        }

        return BehaviorState.Running;
    }
}

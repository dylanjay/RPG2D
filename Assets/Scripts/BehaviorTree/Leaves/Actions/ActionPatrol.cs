using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Patrol", menuName = "Actions/Patrol", order = 1)]
[ShowInNodeEditor("Patrol", false)]
public class ActionPatrol : BehaviorLeaf
{
    [SerializeField]
    Vector2 waypoint1;
    [SerializeField]
    Vector2 waypoint2;
    Hostile hostile;
    bool towardsFirstWaypoint = true;

    public override void Init(GameObject treeHandler)
    {
        base.Init(treeHandler);
    }

    public override void Start()
    {
        hostile.anim.SetTrigger(Hostile.AnimParams.Patrol);
    }

    public override BehaviorState Update()
    {
        Vector2 curPos = hostile.transform.position;
        if (towardsFirstWaypoint)
        {
            hostile.transform.position = Vector2.MoveTowards(curPos, waypoint1, hostile.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint1)
            {
                towardsFirstWaypoint = false;
            }
        }

        else
        {
            hostile.transform.position = Vector2.MoveTowards(curPos, waypoint2, hostile.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint2)
            {
                towardsFirstWaypoint = true;
            }
        }

        return BehaviorState.Running;
    }
}

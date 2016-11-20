using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Patrol", menuName = "Actions/Patrol", order = 1)]
[ShowInNodeEditor(false)]
public class ActionPatrol : BehaviorLeaf
{
    Vector2 waypoint1;
    Vector2 waypoint2;
    Hostile hostile;
    bool towardsFirstWaypoint = true;

    public override void Init(List<ObjectReference> objs)
    {
        base.Init(objs);
        foreach (ObjectReference objRef in objs)
        {
            switch (objRef.name)
            {
                case "waypoint1":
                    waypoint1 = ((Vector2)objRef.obj);
                    break;

                case "waypoint2":
                    waypoint2 = ((Vector2)objRef.obj);
                    break;

                case "hostile":
                    hostile = (Hostile)objRef.obj;
                    break;
            }
        }
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

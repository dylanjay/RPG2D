using UnityEngine;
using System.Collections;
using System;

public class ActionPatrol : Action
{
    Vector2 waypoint1;
    Vector2 waypoint2;
    Hostile hostile;
    bool towards1 = true;

    public ActionPatrol(string name, Vector2 waypoint1, Vector2 waypoint2, Hostile hostile) : base(name)
    {
        this.waypoint1 = waypoint1;
        this.waypoint2 = waypoint2;
        this.hostile =  hostile;
    }

    public override BehaviorState Behave()
    {
        returnState = _Behave();
        return returnState;
    }

    private BehaviorState _Behave()
    {
        Vector2 curPos = hostile.transform.position;
        if (towards1)
        {
            hostile.transform.position = Vector2.MoveTowards(curPos, waypoint1, hostile.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint1)
            {
                towards1 = false;
            }
        }

        else
        {
            hostile.transform.position = Vector2.MoveTowards(curPos, waypoint2, hostile.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint2)
            {
                towards1 = true;
            }
        }

        return BehaviorState.Running;
    }
}

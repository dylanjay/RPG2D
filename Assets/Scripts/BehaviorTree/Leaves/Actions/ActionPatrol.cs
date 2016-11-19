using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Patrol", menuName = "Actions/Patrol", order = 1)]
public class ActionPatrol : BehaviorLeaf
{
    Vector2 waypoint1;
    Vector2 waypoint2;
    Hostile hostile;
    bool towards1 = true;

    /*public ActionPatrol(string name, SerializableVector2 waypoint1, SerializableVector2 waypoint2) : base(name)
    {
        this.waypoint1 = waypoint1.vector2;
        this.waypoint2 = waypoint2.vector2;
    }

    public override void Init(List<ObjectReference> objs)
    {
        base.Init(objs);
        foreach (ObjectReference objRef in objs)
        {
            switch (objRef.name)
            {
                case "waypoint1":
                    waypoint1 = ((SerializableVector2)objRef.obj).vector2;
                    break;

                case "waypoint2":
                    waypoint2 = ((SerializableVector2)objRef.obj).vector2;
                    break;

                case "hostile":
                    hostile = (Hostile)objRef.obj;
                    break;
            }
        }
    }*/

    public ActionPatrol(string name, Vector2 waypoint1, Vector2 waypoint2) : base(name)
    {
        this.waypoint1 = waypoint1;
        this.waypoint2 = waypoint2;
    }

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

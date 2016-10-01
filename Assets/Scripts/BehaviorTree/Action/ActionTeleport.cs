using UnityEngine;
using System.Collections;
using System;

public class ActionTeleport : Action
{
    Vector3 destination;
    Hostile hostile;

    public ActionTeleport(string name, Vector3 destination, Hostile hostile) : base(name)
    {
        this.destination = destination;
        this.hostile = hostile;
    }

    public override BehaviorState Behave()
    {
        returnState = _Behave();
        return returnState;
    }

    private BehaviorState _Behave()
    {
        hostile.transform.position = destination;

        if(hostile.transform.position == destination)
        {
            return BehaviorState.Success;
        }

        return BehaviorState.Running;
    }
}

using UnityEngine;
using System.Collections;
using System;

public class ActionMoveTowardsPlayer : Action
{
    Player player;
    Hostile hostile;

    public ActionMoveTowardsPlayer(string name, Player player, Hostile hostile) : base(name)
    {
        this.player = player;
        this.hostile = hostile;
    }

    public override BehaviorState Behave()
    {
        returnState = _Behave();
        return returnState;
    }

    private BehaviorState _Behave()
    {
        hostile.transform.position = Vector2.MoveTowards(hostile.transform.position, player.transform.position, hostile.moveSpeed.value * Time.deltaTime);

        return BehaviorState.Running;
    }
}

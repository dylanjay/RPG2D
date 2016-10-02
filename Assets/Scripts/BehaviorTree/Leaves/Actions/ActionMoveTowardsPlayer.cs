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
    
    public override void Start()
    {
        hostile.anim.SetTrigger(Hostile.AnimParams.Alert);
    }

    public override BehaviorState Update()
    {
        hostile.transform.position = Vector2.MoveTowards(hostile.transform.position, player.transform.position, hostile.moveSpeed.value * Time.deltaTime);

        return BehaviorState.Running;
    }
}

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
        hostile.AnimParamIDs.Add(name, Animator.StringToHash(name));
    }

    void SetAnimation()
    {
        hostile.anim.SetTrigger(hostile.AnimParamIDs[name]);
    }

    public override BehaviorState Behave()
    {
        SetAnimation();
        returnState = _Behave();
        return returnState;
    }

    private BehaviorState _Behave()
    {
        hostile.transform.position = Vector2.MoveTowards(hostile.transform.position, player.transform.position, hostile.moveSpeed.value * Time.deltaTime);

        return BehaviorState.Running;
    }
}

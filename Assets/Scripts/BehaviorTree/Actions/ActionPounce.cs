using UnityEngine;
using System.Collections;
using System;

public class ActionPounce : Action
{
    Player player;
    Hostile hostile;
    float force = 3.0f;
    Vector2 startPos;
    bool resetPos = true;

    public ActionPounce(string name, Player player, Hostile hostile) : base(name)
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
        Vector2 curPos = hostile.transform.position;
        if (resetPos)
        {
            startPos = curPos;
            resetPos = false;
        }

        hostile.transform.position = Vector2.MoveTowards(curPos, player.transform.position, hostile.moveSpeed.value * force * Time.deltaTime);

        if(hostile.hitPlayer)
        {
            hostile.hitPlayer = false;
            hostile.transform.position = startPos;
            resetPos = true;
            return BehaviorState.Success;
        }

        return BehaviorState.Running;
    }
}

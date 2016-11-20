using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "New Action Pounce", menuName = "Actions/Pounce", order = 3)]
[ShowInNodeEditor(false)]
public class ActionPounce : BehaviorLeaf
{
    Player player;
    Hostile hostile;
    float force = 3.0f;
    Vector2 startPos;

    public void Init(Player player, Hostile hostile)
    {
        this.player = player;
        this.hostile = hostile;
    }

    public override void Start()
    {
        hostile.anim.SetTrigger(Hostile.AnimParams.Pounce);
        startPos = hostile.transform.position;
    }

    public override BehaviorState Update()
    {
        Vector2 curPos = hostile.transform.position;

        hostile.transform.position = Vector2.MoveTowards(curPos, player.transform.position, hostile.moveSpeed.value * force * Time.deltaTime);

        if(hostile.hitPlayer)
        {
            hostile.hitPlayer = false;
            hostile.transform.position = startPos;
            return BehaviorState.Success;
        }

        return BehaviorState.Running;
    }
}

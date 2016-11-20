using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Action Move Towards Player", menuName = "Actions/MoveTowardsPlayer", order = 2)]
[ShowInNodeEditor("Move Towards Player", false)]
public class ActionMoveTowardsPlayer : BehaviorLeaf
{
    Player player;
    Hostile hostile;

    public override void Init(GameObject treeHandler)
    {
        base.Init(treeHandler);
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

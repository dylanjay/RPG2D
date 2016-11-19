using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Action Move Towards Player", menuName = "Actions/MoveTowardsPlayer", order = 2)]
public class ActionMoveTowardsPlayer : BehaviorLeaf
{
    Player player;
    Hostile hostile;

    public ActionMoveTowardsPlayer(string name) : base(name)
    {
    }

    public override void Init(List<ObjectReference> objs)
    {
        base.Init(objs);
        foreach (ObjectReference objRef in objs)
        {
            switch (objRef.name)
            {
                case "player":
                    player = (Player)objRef.obj;
                    break;

                case "hostile":
                    hostile = (Hostile)objRef.obj;
                    break;
            }
        }
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

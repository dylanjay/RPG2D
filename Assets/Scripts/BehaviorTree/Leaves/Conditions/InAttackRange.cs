using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Condition In Attack Range", menuName = "Conditions/InAttackRange", order = 1)]
public class InAttackRange : BehaviorLeaf
{
    Player player;
    Hostile hostile;
    float distance;

    public InAttackRange(string name, float distance) : base(name)
    {
        this.distance = distance;
    }

    public void Init(Player player, Hostile hostile, float distance)
    {
        this.player = player;
        this.hostile = hostile;
        this.distance = distance;
    }

    public override void Start()
    {

    }

    public override BehaviorState Update()
    {
        if (Vector2.Distance(hostile.transform.position, player.transform.position) <= distance)
        {
            return BehaviorState.Success;
        }

        else
        {
            return BehaviorState.Failure;
        }
    }
}

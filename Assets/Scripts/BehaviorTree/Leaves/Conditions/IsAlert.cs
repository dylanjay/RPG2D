using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Condition Is Alert", menuName = "Conditions/IsAlert", order = 0)]
public class IsAlert : BehaviorLeaf
{
    Player player;
    Hostile hostile;
    float distance;

    public IsAlert(string name, float distance) : base(name)
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

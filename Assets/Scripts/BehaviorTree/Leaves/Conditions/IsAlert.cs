using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Condition Is Alert", menuName = "Conditions/IsAlert", order = 0)]
[ShowInNodeEditor("IsAlert", false)]
public class IsAlert : BehaviorLeaf
{
    Player player;
    Hostile hostile;
    [SerializeField]
    float alertRadius = 1.0f;

    public override void Init(Dictionary<string, GameObject> referenceDict)
    {
        base.Init(referenceDict);
        player = referenceDict[MemberInfoGetting.GetMemberName(() => player) + "Reference"].GetComponent<Player>();
        hostile = referenceDict[MemberInfoGetting.GetMemberName(() => hostile) + "Reference"].GetComponent<Hostile>();
    }

    public override void Start()
    {

    }

    public override BehaviorState Update()
    {
        if (Vector2.Distance(hostile.transform.position, player.transform.position) <= alertRadius)
        {
            return BehaviorState.Success;
        }

        else
        {
            return BehaviorState.Failure;
        }
    }
}

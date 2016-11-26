using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Condition In Attack Range", menuName = "Conditions/InAttackRange", order = 1)]
[ShowInNodeEditor("InAttackRange", false)]
public class InAttackRange : BehaviorLeaf
{
    Player player;
    Hostile hostile;
    [SerializeField]
    float attackRadius = 1.0f;

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
        if (Vector2.Distance(hostile.transform.position, player.transform.position) <= attackRadius)
        {
            return BehaviorState.Success;
        }

        else
        {
            return BehaviorState.Failure;
        }
    }
}

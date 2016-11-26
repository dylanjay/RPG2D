using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TestNodeTree : BehaviorTree
{
    [SerializeField]
    private GameObject playerReference;
    [SerializeField]
    private GameObject hostileReference;

    public override void Start()
    {
        referenceDict.Add(MemberInfoGetting.GetMemberName(() => playerReference), playerReference);
        referenceDict.Add(MemberInfoGetting.GetMemberName(() => hostileReference), hostileReference);
        base.Start();
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Wait", menuName = "Actions/Wait", order = 0)]
[ShowInNodeEditor("Wait For Seconds", false)]
public class ActionWait : BehaviorLeaf
{
    float time = 0.0f;
    [SerializeField]
    float maxTime = 1.0f;
    Hostile hostile;

    public override void Init(Dictionary<string, GameObject> referenceDict)
    {
        base.Init(referenceDict);
        hostile = referenceDict[MemberInfoGetting.GetMemberName(() => hostile) + "Reference"].GetComponent<Hostile>();
    }

    void Reset()
    {
        time = 0.0f;
    }

    public override void Start()
    {
        hostile.anim.SetTrigger(Hostile.AnimParams.Idle);
    }

    public override BehaviorState Update()
    {
        if(time >= maxTime)
        {
            Reset();
            return BehaviorState.Success;
        }

        time += Time.deltaTime;

        return BehaviorState.Running;
    }
}

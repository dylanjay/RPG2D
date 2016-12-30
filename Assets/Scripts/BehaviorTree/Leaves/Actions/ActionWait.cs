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
    SharedHostile This;

    public override void Init(Dictionary<string, object> sharedVarDict)
    {
        This.Value = ((GameObject)sharedVarDict[This.name]).GetComponent<Hostile>();
    }

    void Reset()
    {
        time = 0.0f;
    }

    public override void Start()
    {
        This.Value.anim.SetTrigger(Hostile.AnimParams.Idle);
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

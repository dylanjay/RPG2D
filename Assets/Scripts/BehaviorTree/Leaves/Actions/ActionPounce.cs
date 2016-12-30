using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Pounce", menuName = "Actions/Pounce", order = 3)]
[ShowInNodeEditor("Pounce", false)]
public class ActionPounce : BehaviorLeaf
{
    [SerializeField]
    private SharedTransform target;
    private SharedHostile This;
    [SerializeField]
    private float force = 3.0f;
    private Vector2 startPos;

    public override void Init(Dictionary<string, object> sharedVarDict)
    {
        target = (SharedTransform)sharedVarDict[target.name];
        This = ((SharedHostile)sharedVarDict[This.name]);
    }

    public override void Start()
    {
        This.Value.anim.SetTrigger(Hostile.AnimParams.Pounce);
        startPos = This.Value.transform.position;
    }

    public override BehaviorState Update()
    {
        Vector2 curPos = This.Value.transform.position;

        This.Value.transform.position = Vector2.MoveTowards(curPos, target.Value.transform.position, This.Value.moveSpeed.value * force * Time.deltaTime);

        if(This.Value.hitPlayer)
        {
            This.Value.hitPlayer = false;
            This.Value.transform.position = startPos;
            return BehaviorState.Success;
        }

        return BehaviorState.Running;
    }
}

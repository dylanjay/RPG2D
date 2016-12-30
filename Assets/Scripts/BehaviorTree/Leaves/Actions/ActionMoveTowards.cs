using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Move Towards", menuName = "Actions/MoveTowards", order = 2)]
[ShowInNodeEditor("Move Towards", false)]
public class ActionMoveTowards : BehaviorLeaf
{
    [SerializeField]
    private SharedTransform target;
    private SharedHostile This;

    public override void Init(Dictionary<string, object> sharedVarDict)
    {
        target = (SharedTransform)sharedVarDict[target.name];
        //This = (SharedHostile)sharedVarDict["This"]);
    }

    public override void Start()
    {
        This.Value.anim.SetTrigger(Hostile.AnimParams.Alert);
    }

    public override BehaviorState Update()
    {
        This.Value.transform.position = Vector2.MoveTowards(This.Value.transform.position, target.Value.position, This.Value.moveSpeed.value * Time.deltaTime);

        return BehaviorState.Running;
    }
}
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Action Move Towards Player", menuName = "Actions/MoveTowardsPlayer", order = 2)]
[ShowInNodeEditor("Move Towards Player", false)]
public class ActionMoveTowardsPlayer : BehaviorLeaf
{
    [SerializeField]
    private SharedTransform player;
    private SharedHostile This;

    public override void Init(Dictionary<string, object> sharedVarDict)
    {
        player.Value = (Transform)sharedVarDict[player.name];
        This.Value = ((GameObject)sharedVarDict[This.name]).GetComponent<Hostile>();
    }

    public override void Start()
    {
        This.Value.anim.SetTrigger(Hostile.AnimParams.Alert);
    }

    public override BehaviorState Update()
    {
        This.Value.transform.position = Vector2.MoveTowards(This.Value.transform.position, player.Value.position, This.Value.moveSpeed.value * Time.deltaTime);

        return BehaviorState.Running;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Action Patrol", menuName = "Actions/Patrol", order = 1)]
[ShowInNodeEditor("Patrol", false)]
public class ActionPatrol : BehaviorLeaf
{
    [SerializeField]
    private Vector2 waypoint1;
    [SerializeField]
    private Vector2 waypoint2;
    private SharedHostile This;
    private bool towardsFirstWaypoint = true;

    public override void Init(Dictionary<string, object> sharedVarDict)
    {
        This.Value = ((GameObject)sharedVarDict[This.name]).GetComponent<Hostile>();
    }

    public override void Start()
    {
        This.Value.anim.SetTrigger(Hostile.AnimParams.Patrol);
    }

    public override BehaviorState Update()
    {
        Vector2 curPos = This.Value.transform.position;
        if (towardsFirstWaypoint)
        {
            This.Value.transform.position = Vector2.MoveTowards(curPos, waypoint1, This.Value.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint1)
            {
                towardsFirstWaypoint = false;
            }
        }

        else
        {
            This.Value.transform.position = Vector2.MoveTowards(curPos, waypoint2, This.Value.moveSpeed.value * Time.deltaTime);
            if (curPos == waypoint2)
            {
                towardsFirstWaypoint = true;
            }
        }

        return BehaviorState.Running;
    }
}

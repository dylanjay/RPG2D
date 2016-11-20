using UnityEngine;
using System.Collections;

public class BehaviorWait : BehaviorDecorator
{
    float time = 0.0f;
    float maxTime;

    void Reset()
    {
        time = 0.0f;
    }

    public override BehaviorState Behave()
    {
        returnState = _Behave();
        return returnState;
    }

    private BehaviorState _Behave()
    {
        if (time >= maxTime)
        {
            BehaviorState childState = childBehavior.Behave();
            Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");
            if (childState == BehaviorState.Success)
            {
                Reset();
            }
            return childState;
            //return BehaviorState.Success?
        }

        time += Time.deltaTime;

        return BehaviorState.Running;
    }
}

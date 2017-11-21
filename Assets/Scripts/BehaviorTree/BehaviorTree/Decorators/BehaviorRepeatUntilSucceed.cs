using UnityEngine;
using System.Collections;
using System;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    /// <summary>
    /// A short circuiting repeater. Behaves like a conditional while statement:
    /// 
    /// Reprocesses child behavior until the child returns success and subsequently return success
    /// </summary>

    [ShowInNodeEditor("Repeat Until Succeed")]
    public class BehaviorRepeatUntilSucceed : BehaviorDecorator
    {
        bool succeed = false;

        public override BehaviorState Behave()
        {
            returnState = _Behave();
            return returnState;
        }

        /// <summary>
        /// A helper function for the meat of the behavior
        /// </summary>
        /// <returns></returns>
        private BehaviorState _Behave()
        {
            if (succeed)
            {
                return BehaviorState.Success;
            }

            BehaviorState childState = childBehavior.Behave();
            Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");

            switch (childState)
            {
                case BehaviorState.Success:
                    succeed = true;
                    return BehaviorState.Success;

                case BehaviorState.Failure:
                    return BehaviorState.Running;

                default:
                    return childState;
            }
        }
    }
}

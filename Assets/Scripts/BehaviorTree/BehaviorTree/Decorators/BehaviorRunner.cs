using UnityEngine;
using System.Collections;
using System;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    /// <summary>
    /// A short circuiting runner. Behaves like a conditional running statement:
    /// 
    /// Always returns Running regardless of child's return value
    /// </summary>

    [ShowInNodeEditor("Runner")]
    public class BehaviorRunner : BehaviorDecorator
    {
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
            BehaviorState childState = childBehavior.Behave();
            Debug.Assert(childState != BehaviorState.None, "Error: Child behavior \"" + childBehavior.name + "\" of behavior \"" + name + "\" has no defined behavior.");

            return BehaviorState.Running;
        }
    }
}

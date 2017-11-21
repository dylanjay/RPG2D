using UnityEngine;
using System.Collections;
using System;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    /// <summary>
    /// A short circuiting sequencer. Behaves like a conditional statement:
    /// 
    /// Returns Failure/Error/Running on the first failure/error/running. Will not run any behaviors after that.
    /// Returns Success if and only if each child has returned success.
    /// </summary>
    [ShowInNodeEditor("While")]
    public class BehaviorWhile : BehaviorComposite
    {
        private const int CONDTION_BEHAVIOR = 0;
        private const int ACTION_BEHAVIOR = 1;

        private BehaviorComponent condition { get { return childBehaviors[CONDTION_BEHAVIOR]; } }
        private BehaviorComponent action { get { return childBehaviors[ACTION_BEHAVIOR]; } }

        public override void Initialize(string name, BehaviorComponent[] childBehaviors = null)
        {
            if (childBehaviors != null)
            {
                Debug.Assert(childBehaviors.Length == 2, "Error: BehaviorWhile's can only have two children. The first is a condition, the second is the action.");
            }
            base.Initialize(name, childBehaviors);
        }

        public override BehaviorState Behave()
        {
            returnState = _Behave();
            return returnState;
        }

        /// <summary>
        /// A helper function for the meat of the behavior.
        /// </summary>
        /// <returns></returns>
        private BehaviorState _Behave()
        {
            if (condition.Behave() == BehaviorState.Failure)
            {
                //We return success because the while loop has completed.

                Debug.Log("Failure => Success");
                return BehaviorState.Success;
            }
            else
            {
                BehaviorState temp = action.Behave();
                if (temp != BehaviorState.Error)
                {
                    return BehaviorState.Running;
                }
                else
                {
                    return BehaviorState.Error;
                }
            }
        }
    }
}
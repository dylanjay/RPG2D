using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    public class DialogueNode : BehaviorNodeBase
    {
        protected override void Initialize()
        {
            input = CreateInstance<MultiPort>();
            input.Init(this);
            input.hideFlags = HideFlags.HideInHierarchy;

            output = CreateInstance<MultiPort>();
            output.Init(this);
            output.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}

using UnityEngine;
using System;
using System.Collections.ObjectModel;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    public class NodeComposite : BehaviorNodeBase
    {
        protected override void Initialize()
        {
            if (input != null && output != null)
            {
                return;
            }

            input = CreateInstance<SingleNodePort>();
            input.Init(this);
            input.hideFlags = HideFlags.HideInHierarchy;

            output = CreateInstance<MultiPort>();
            output.Init(this);
            output.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}
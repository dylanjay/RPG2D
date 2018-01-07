using UnityEngine;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using UnityEditor;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    public class NodeDecorator : BehaviorNodeBase
    {
        protected override void Initialize()
        {
            if (input != null || output != null)
            {
                Debug.LogError("Somehow the ports have already been initialized...");
            }
            
            input = CreateInstance<SingleNodePort>();
            input.Init(this);
            input.hideFlags = HideFlags.HideInHierarchy;

            output = CreateInstance<SingleNodePort>();
            output.Init(this);
            output.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}

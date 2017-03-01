using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public abstract class BehaviorNodeBase : NodeBase
    {
        public new NodeBehaviorTree parentGraph
        {
            get { return (NodeBehaviorTree)base.parentGraph; }
            set { base.parentGraph = value; }
        }
    }
}

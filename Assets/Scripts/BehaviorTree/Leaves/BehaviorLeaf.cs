﻿using UnityEngine;
using Benco.BehaviorTree;
using System.Collections.Generic;

namespace Benco.BehaviorTree
{
    public abstract class BehaviorLeaf : BehaviorComponent
    {
        private bool callOnStart = true;

        [SerializeField]
        private SharedTransform _transform;
        protected Transform transform
        {
            get
            {
                return _transform.value;
            }
            set
            {
                _transform.value = value;
            }
        }

        [SerializeField]
        private SharedHostile _entity;
        protected Hostile entity
        {
            get
            {
                return _entity.value;
            }
            set
            {
                _entity.value = value;
            }
        }

        public sealed override BehaviorState Behave()
        {
            if (callOnStart)
            {
                OnStart();
                callOnStart = false;
            }

            BehaviorState ret = Update();

            if (ret != BehaviorState.Running)
            {
                OnEnd();
                callOnStart = true;
            }

            return ret;
        }


        public void Reset()
        {
            OnReset();
            callOnStart = true;
        }

        public virtual void OnStart() { }
        public virtual void OnEnd() { }
        public virtual void OnReset() { }
        public abstract BehaviorState Update();
    }
}
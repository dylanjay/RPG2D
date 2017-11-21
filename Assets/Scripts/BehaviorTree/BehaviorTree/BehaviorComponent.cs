using Benco.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.BehaviorTree
{
    public enum BehaviorState
    {
        Failure,
        Running,
        Success,
        Error,
        None
    }

    public delegate BehaviorState Behavior();

    public abstract class BehaviorComponent : ScriptableObject, INode
    {
        protected BehaviorState _returnState = BehaviorState.None;

        /// <summary>
        /// Creates a default BehaviorComponent of type T. 
        /// Use child class implementations of CreateInstance to initialize derived class members.
        /// </summary>
        /// <typeparam name="T">The type of the BehaviorComponent.</typeparam>
        /// <param name="name">The name given to the component.</param>
        /// <returns>The default implementation of the BehaviorComponent of type T.</returns>
        public static BehaviorComponent CreateComponent(System.Type componentType, string name = "")
        {
            BehaviorComponent behaviorComponent = (BehaviorComponent)ScriptableObject.CreateInstance(componentType);
            behaviorComponent.hideFlags = HideFlags.HideInHierarchy;
            behaviorComponent.Instantiate(name);
            return behaviorComponent;
        }

        /// <summary>
        /// Most recent return state.
        /// </summary>
        public BehaviorState returnState
        {
            get { return _returnState; }
            protected set { _returnState = value; }
        }

        protected virtual void Instantiate(string name)
        {
            if (name == "")
            {
                //Gets the name of the type and removes the word "Behavior" from the beginning of the string.
                object[] attributeArray = GetType().GetCustomAttributes(typeof(ShowInNodeEditorAttribute), false);
                if (attributeArray.Length > 0)
                {
                    this.name = ((ShowInNodeEditorAttribute[])attributeArray)[0].displayName;
                }
                else
                {
                    this.name = GetType().ToString();
                }
            }
            else
            {
                this.name = name;
            }
        }

        /// <summary>
        /// perform the behavior
        /// </summary>
        public abstract BehaviorState Behave();

        public virtual IEnumerator<BehaviorComponent> GetChildren()
        {
            yield break;
        }

        public virtual void OnAwake() { }
    }
}

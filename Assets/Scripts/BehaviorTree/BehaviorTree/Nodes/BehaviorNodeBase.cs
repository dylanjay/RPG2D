using UnityEngine;
using Benco.Graph;

namespace Benco.BehaviorTree
{
    public abstract class BehaviorNodeBase : NodeBase
    {
        public BehaviorComponent behaviorComponent;

        [System.Serializable]
        protected class ChoiceDictionary : SerializableDictionary<string, SharedVariable> { }

        /// <summary>
        /// Key: The name of the field
        /// Value: The sharedVariable chosen
        /// </summary>
        [SerializeField]
        protected ChoiceDictionary choices = new ChoiceDictionary();

        public new NodeBehaviorTree parentGraph
        {
            get { return (NodeBehaviorTree)base.parentGraph; }
            set { base.parentGraph = value; }
        }

        public void OnEnable()
        {
            if (input == null)
            {
                input = CreateInstance<SingleNodePort>();
                input.Init(this);
                input.hideFlags = HideFlags.HideInHierarchy;
            }
            if (output == null)
            {
                output = CreateInstance<MultiPort>();
                output.Init(this);
                output.hideFlags = HideFlags.HideInHierarchy;
            }
        }
    }
}

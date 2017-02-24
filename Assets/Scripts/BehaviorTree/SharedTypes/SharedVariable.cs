using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.BehaviorTree
{
    /// <summary>
    /// A base class to help with the typing in other scripts.
    /// Don't inherit from this unless you want to mess your codebase up.
    /// </summary>
    [System.Serializable]
    public abstract class SharedVariable : ScriptableObject
    {
        public abstract System.Type sharedType { get; }
    }
    
    [System.Serializable]
    public sealed class NullSharedVariable : SharedVariable
    {
        public override System.Type sharedType { get { return typeof(void); } }
    }
    
    [System.Serializable]
    public class SharedVariable<T> : SharedVariable
    {
        [SerializeField, HideInInspector]
        public T value;

        protected SharedVariable()
        {

        }

        protected SharedVariable(T value)
        {
            this.value = value;
        }

        public static implicit operator T(SharedVariable<T> sharedVariable)
        {
            return sharedVariable.value;
        }

        public static implicit operator SharedVariable<T>(T value)
        {
            SharedVariable<T> ret = ScriptableObject.CreateInstance<SharedVariable<T>>();
            ret.value = value;
            return ret;
        }

        public override System.Type sharedType
        {
            get { return typeof(T); }
        }
    }


    public class TypeNameOverrideAttribute : System.Attribute
    {
        public string newDisplayName;
    }

    //TODO: Move these elsewhere!

    public class SharedHostile : SharedVariable<Hostile> { }

    public class SharedPlayer : SharedVariable<Player> { }

    public class SharedTransformList : SharedVariable<List<Transform>> { }
}







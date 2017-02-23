using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.BehaviorTree
{
    /// <summary>
    /// A base class to help with the typing in other scripts. Effectively an alias for ScriptableObject that only 
    /// Scriptable Objects inherit from. Don't inherit from this unless you want to mess your codebase up.
    /// </summary>
    [System.Serializable]
    public abstract class SharedVariable : ScriptableObject
    {
        public abstract System.Type sharedType { get; }
    }

    [System.Serializable]
    public class NullSharedVariable : SharedVariable
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

    //public class SharedColor : SharedVariable<Color> { }

    //public class SharedFloat : SharedVariable<float> { }

    //public class SharedGameObject : SharedVariable<GameObject> { }

    //public class SharedGameObjectList : SharedVariable<List<GameObject>> { }

    public class SharedHostile : SharedVariable<Hostile> { }

    //public class SharedHostileList : SharedVariable<List<Hostile>> { }

    //public class SharedInt : SharedVariable<int> { }

    //public class SharedMaterial : SharedVariable<Material> { }

    //public class SharedObject : SharedVariable<Object> { }

    //public class SharedObjectList : SharedVariable<List<Object>> { }

    public class SharedPlayer : SharedVariable<Player> { }

    //public class SharedQuaternion : SharedVariable<Quaternion> { }

    //public class SharedRect : SharedVariable<Rect> { }

    //public class SharedString : SharedVariable<string> { }

    public class SharedTransformList : SharedVariable<List<Transform>> { }

    //public class SharedVector2 : SharedVariable<Vector2> { }

    //public class SharedVector3 : SharedVariable<Vector3> { }

    //public class SharedVector4 : SharedVariable<Vector4> { }
}







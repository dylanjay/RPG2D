using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedVariable<T>
{
    public T value;
    public string name;

    protected SharedVariable()
    {

    }

    protected SharedVariable(T value)
    {
        this.value = value;
    }

    public static implicit operator T (SharedVariable<T> sharedVariable)
    {
        return sharedVariable.value;
    }

    public static implicit operator SharedVariable<T>(T value)
    {
        return new SharedVariable<T>(value);
    }

    public System.Type sharedType
    {
        get { return typeof(T); }
    }
}

[System.Serializable]
public class SharedBool : SharedVariable<bool> {}

[System.Serializable]
public class SharedColor : SharedVariable<Color> {}

[System.Serializable]
public class SharedFloat : SharedVariable<float> {}

[System.Serializable]
public class SharedGameObject : SharedVariable<GameObject> {}

[System.Serializable]
public class SharedGameObjectList : SharedVariable<List<GameObject>> {}

[System.Serializable]
public class SharedHostile : SharedVariable<Hostile> {}

[System.Serializable]
public class SharedHostileList : SharedVariable<List<Hostile>> {}

[System.Serializable]
public class SharedInt : SharedVariable<int> {}

[System.Serializable]
public class SharedMaterial : SharedVariable<Material> {}

[System.Serializable]
public class SharedObject : SharedVariable<Object> {}

[System.Serializable]
public class SharedObjectList : SharedVariable<List<Object>> {}

[System.Serializable]
public class SharedPlayer : SharedVariable<Player> {}

[System.Serializable]
public class SharedQuaternion : SharedVariable<Quaternion> {}

[System.Serializable]
public class SharedRect : SharedVariable<Rect> {}

[System.Serializable]
public class SharedString : SharedVariable<string> {}

[System.Serializable]
public class SharedTransform : SharedVariable<Transform> {}

[System.Serializable]
public class SharedTransformList : SharedVariable<List<Transform>> {}

[System.Serializable]
public class SharedVector2 : SharedVariable<Vector2> {}

[System.Serializable]
public class SharedVector3 : SharedVariable<Vector3> {}

[System.Serializable]
public class SharedVector4 : SharedVariable<Vector4> { }










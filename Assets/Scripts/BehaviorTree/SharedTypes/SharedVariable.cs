using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedVariable <T>
{
    public SharedVariable()
    {

    }

    public SharedVariable(T value)
    {
        Value = value;
    }

    public T Value;
    public string name;
}

public class SharedBool : SharedVariable<bool>
{
    public SharedBool()
    {
    }

    public SharedBool(bool value) : base(value)
    {
    }

    public static implicit operator SharedBool(bool value) { return new SharedBool { Value = value }; }
}

public class SharedColor : SharedVariable<Color>
{
    public SharedColor()
    {
    }

    public SharedColor(Color value) : base(value)
    {
    }

    public static implicit operator SharedColor(Color value) { return new SharedColor { Value = value }; }
}

public class SharedFloat : SharedVariable<float>
{
    public SharedFloat()
    {
    }

    public SharedFloat(float value) : base(value)
    {
    }

    public static implicit operator SharedFloat(float value) { return new SharedFloat { Value = value }; }
}

public class SharedGameObject : SharedVariable<GameObject>
{
    public SharedGameObject()
    {
    }

    public SharedGameObject(GameObject value) : base(value)
    {
    }

    public static implicit operator SharedGameObject(GameObject value) { return new SharedGameObject { Value = value }; }
}

public class SharedGameObjectList : SharedVariable<List<GameObject>>
{
    public SharedGameObjectList()
    {
    }

    public SharedGameObjectList(List<GameObject> value) : base(value)
    {
    }

    public static implicit operator SharedGameObjectList(List<GameObject> value) { return new SharedGameObjectList { Value = value }; }
}

public class SharedHostile : SharedVariable<Hostile>
{
    public SharedHostile()
    {
    }

    public SharedHostile(Hostile value) : base(value)
    {
    }

    public static implicit operator SharedHostile(Hostile value) { return new SharedHostile { Value = value }; }
}

public class SharedHostileList : SharedVariable<List<Hostile>>
{
    public SharedHostileList()
    {
    }

    public SharedHostileList(List<Hostile> value) : base(value)
    {
    }

    public static implicit operator SharedHostileList(List<Hostile> value) { return new SharedHostileList { Value = value }; }
}

public class SharedInt : SharedVariable<int>
{
    public SharedInt()
    {
    }

    public SharedInt(int value) : base(value)
    {
    }

    public static implicit operator SharedInt(int value) { return new SharedInt { Value = value }; }
}

public class SharedMaterial : SharedVariable<Material>
{
    public SharedMaterial()
    {
    }

    public SharedMaterial(Material value) : base(value)
    {
    }

    public static implicit operator SharedMaterial(Material value) { return new SharedMaterial { Value = value }; }
}

public class SharedObject : SharedVariable<Object>
{
    public SharedObject()
    {
    }

    public SharedObject(Object value) : base(value)
    {
    }

    public static implicit operator SharedObject(Object value) { return new SharedObject { Value = value }; }
}

public class SharedObjectList : SharedVariable<List<Object>>
{
    public SharedObjectList()
    {
    }

    public SharedObjectList(List<Object> value) : base(value)
    {
    }

    public static implicit operator SharedObjectList(List<Object> value) { return new SharedObjectList { Value = value }; }
}

public class SharedPlayer : SharedVariable<Player>
{
    public SharedPlayer()
    {
    }

    public SharedPlayer(Player value) : base(value)
    {
    }

    public static implicit operator SharedPlayer(Player value) { return new SharedPlayer { Value = value }; }
}

public class SharedQuaternion : SharedVariable<Quaternion>
{
    public SharedQuaternion()
    {
    }

    public SharedQuaternion(Quaternion value) : base(value)
    {
    }

    public static implicit operator SharedQuaternion(Quaternion value) { return new SharedQuaternion { Value = value }; }
}

public class SharedRect : SharedVariable<Rect>
{
    public SharedRect()
    {
    }

    public SharedRect(Rect value) : base(value)
    {
    }

    public static implicit operator SharedRect(Rect value) { return new SharedRect { Value = value }; }
}

public class SharedString : SharedVariable<string>
{
    public SharedString()
    {
    }

    public SharedString(string value) : base(value)
    {
    }

    public static implicit operator SharedString(string value) { return new SharedString { Value = value }; }
}

public class SharedTransform : SharedVariable<Transform>
{
    public SharedTransform()
    {
    }

    public SharedTransform(Transform value) : base(value)
    {
    }

    public static implicit operator SharedTransform(Transform value) { return new SharedTransform { Value = value }; }
}

public class SharedTransformList : SharedVariable<List<Transform>>
{
    public SharedTransformList()
    {
    }

    public SharedTransformList(List<Transform> value) : base(value)
    {
    }

    public static implicit operator SharedTransformList(List<Transform> value) { return new SharedTransformList { Value = value }; }
}

public class SharedVector2 : SharedVariable<Vector2>
{
    public SharedVector2()
    {
    }

    public SharedVector2(Vector2 value) : base(value)
    {
    }

    public static implicit operator SharedVector2(Vector2 value) { return new SharedVector2 { Value = value }; }
}

public class SharedVector3 : SharedVariable<Vector3>
{
    public SharedVector3()
    {
    }

    public SharedVector3(Vector3 value) : base(value)
    {
    }

    public static implicit operator SharedVector3(Vector3 value) { return new SharedVector3 { Value = value }; }
}

public class SharedVector4 : SharedVariable<Vector4>
{
    public SharedVector4()
    {
    }

    public SharedVector4(Vector4 value) : base(value)
    {
    }

    public static implicit operator SharedVector4(Vector4 value) { return new SharedVector4 { Value = value }; }
}










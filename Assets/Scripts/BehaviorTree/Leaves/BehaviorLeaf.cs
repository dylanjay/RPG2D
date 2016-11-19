using UnityEngine;
using System.Collections.Generic;

public class ObjectReference
{
    public object obj { get; set; }
    public string name { get; set; }

    public ObjectReference(object obj, string name)
    {
        this.obj = obj;
        this.name = name;
    }
}

[System.Serializable]
public abstract class BehaviorLeaf : BehaviorComponent
{
    public BehaviorLeaf(string name) : base(name) { }

    private float lastTime = float.MinValue;

    public virtual void Init(List<ObjectReference> objs)
    {

    }

    public sealed override BehaviorState Behave()
    {
        if (Time.time > Time.deltaTime * 1.5f + lastTime)
        {
            Start();
        }
        lastTime = Time.time;

        return Update();
    }

    public abstract void Start();

    public abstract BehaviorState Update();
}

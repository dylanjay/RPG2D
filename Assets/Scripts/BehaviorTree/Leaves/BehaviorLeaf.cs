using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class BehaviorLeaf : BehaviorComponent
{
    private float lastTime = float.MinValue;
    public List<SerializableDictionaryPair> sharedVarDictPairs = new List<SerializableDictionaryPair>();

    public virtual void Init(Dictionary<string, object> sharedVarDict)
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

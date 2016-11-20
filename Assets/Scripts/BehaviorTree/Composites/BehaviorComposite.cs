using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BehaviorComposite : BehaviorComponent
{
    public BehaviorComponent[] childBehaviors;

    public static BehaviorComposite CreateInstance<T>(string name = "", BehaviorComponent[] childBehaviors = null) where T : BehaviorComposite
    {
        BehaviorComposite behaviorComposite = ScriptableObject.CreateInstance<T>();
        if(name == "")
        {
            //Gets the name of the type and removes the word "Behavior" from the beginning of the string.
            behaviorComposite.GetType().ToString().Substring(8);
        }
        behaviorComposite.Initialize(name, childBehaviors);
        return behaviorComposite;
    }

    protected override void Initialize(string name)
    {
        Initialize(name, null);
    }

    protected virtual void Initialize(string name, BehaviorComponent[] childBehaviors = null)
    {
        base.Initialize(name);
        this.childBehaviors = childBehaviors;
    }

    public void Shuffle()
    {
        for (int i = 0; i < childBehaviors.Length; i++)
        {
            BehaviorComponent temp = childBehaviors[i];
            int rand = UnityEngine.Random.Range(i, childBehaviors.Length);
            childBehaviors[i] = childBehaviors[rand];
            childBehaviors[rand] = temp;
        }
    }
}

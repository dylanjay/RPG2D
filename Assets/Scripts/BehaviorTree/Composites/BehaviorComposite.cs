﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BehaviorComposite : BehaviorComponent
{
    [HideInInspector]
    public BehaviorComponent[] childBehaviors;

    protected override void Initialize(string name)
    {
        Initialize(name, null);
    }

    public virtual void Initialize(string name, BehaviorComponent[] childBehaviors = null)
    {
        base.Initialize(name);
        this.childBehaviors = childBehaviors;
    }

    //TODO: This function is currently being called during the editor.
    //We should probably move the shuffle into an in-game Initialize(),
    //rather than an in-editor Initialize().
    public void Shuffle()
    {
        if(childBehaviors != null)
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
}

﻿using UnityEngine;

public enum BehaviorState
{
    Failure,
    Running,
    Success,
    Error,
    None
}

public delegate BehaviorState Behavior();

[System.Serializable]
public abstract class BehaviorComponent : ScriptableObject{
    
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
        behaviorComponent.Initialize(name);
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

    protected virtual void Initialize(string name)
    {
        if (name == "")
        {
            //Gets the name of the type and removes the word "Behavior" from the beginning of the string.
            this.name = GetType().ToString().Substring(8);
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

}

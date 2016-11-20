using UnityEngine;

public abstract class BehaviorDecorator : BehaviorComponent {

    public BehaviorComponent childBehavior;

    public static T CreateInstance<T>(string name = "", BehaviorComponent childBehavior = null) where T : BehaviorDecorator
    {
        T behaviorDecorator = ScriptableObject.CreateInstance<T>();
        if (name == "")
        {
            //Gets the name of the type and removes the word "Behavior" from the beginning of the string.
            behaviorDecorator.GetType().ToString().Substring(8);
        }
        behaviorDecorator.Initialize(name, childBehavior);
        return behaviorDecorator;
    }

    protected override void Initialize(string name)
    {
        Initialize(name, null);
    }

    protected virtual void Initialize(string name, BehaviorComponent childBehavior = null)
    {
        base.Initialize(name);
        this.childBehavior = childBehavior;
    }

}

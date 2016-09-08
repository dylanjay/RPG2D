using UnityEngine;

public enum BehaviorState
{
    Failure,
    Running,
    Success,
    Error,
    None
}

public delegate BehaviorState Behavior();

public abstract class BehaviorComponent {
    
    protected BehaviorState _returnState = BehaviorState.None;

    public string name = "";

    /// <summary>
    /// Most recent return state.
    /// </summary>
    public BehaviorState returnState
    {
        get { return _returnState; }
        protected set { _returnState = value; }
    }

    public BehaviorComponent(string name) { this.name = name; }

    /// <summary>
    /// perform the behavior
    /// </summary>
    public abstract BehaviorState Behave();

}

public abstract class BehaviorDecorator : BehaviorComponent {

    public BehaviorComponent childBehavior;

    public BehaviorDecorator(string name) : base(name) { }
    public BehaviorDecorator(string name, BehaviorComponent child) : base(name)
    {
        childBehavior = child;
    }

}

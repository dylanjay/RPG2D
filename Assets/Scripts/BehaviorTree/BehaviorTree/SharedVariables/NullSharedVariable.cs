using Benco.BehaviorTree;

[System.Serializable]
public sealed class NullSharedVariable : SharedVariable
{
    public override System.Type sharedType { get { return typeof(void); } }
}

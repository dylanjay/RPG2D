using UnityEngine;
using System.Collections.Generic;

public abstract class BehaviorTree : MonoBehaviour
{
    protected BehaviorComponent tree;

    protected Dictionary<string, object> sharedVarDict = new Dictionary<string, object>();
    protected Dictionary<System.Type, List<string>> names = new Dictionary<System.Type, List<string>>();

    void InitializeTree(BehaviorComponent node)
    {
        node.OnAwake();
        IEnumerator<BehaviorComponent> children = node.GetChildren();
        while(children.MoveNext())
        {
            InitializeTree(children.Current);
        }
    }

    public virtual void Awake()
    {
        InitializeTree(tree);
    }
	
	void Update()
    {
        tree.Behave();
	}
}

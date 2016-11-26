using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public abstract class BehaviorTree : MonoBehaviour
{
    [SerializeField]
    protected BehaviorComponent tree;

    protected Dictionary<string, GameObject> referenceDict = new Dictionary<string, GameObject>();

    void InitializeLeaf(BehaviorLeaf leaf)
    {
        leaf.Init(referenceDict);
    }

    void InitializeTree(BehaviorComponent node)
    {
        EditorUtility.SetDirty(node);
        if (node.GetType().IsSubclassOf(typeof(BehaviorComposite)) || node.GetType().IsSubclassOf(typeof(BehaviorDecorator)))
        {
            BehaviorComposite composite = (BehaviorComposite)node;

            for (int i = 0; i < composite.childBehaviors.Length; i++)
            {
                InitializeTree(composite.childBehaviors[i]);
            }
        }
        else if (node.GetType().IsSubclassOf(typeof(BehaviorLeaf)))
        {
            InitializeLeaf((BehaviorLeaf)node);
        }
    }

    public virtual void Start ()
    {
        InitializeTree(tree);
    }
	
	void Update ()
    {
        tree.Behave();
	}
}

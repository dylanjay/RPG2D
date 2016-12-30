using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public abstract class BehaviorTree : MonoBehaviour
{
    protected BehaviorComponent tree;

    protected Dictionary<string, object> sharedVarDict = new Dictionary<string, object>();

    void InitializeLeaf(BehaviorLeaf leaf)
    {
        foreach(SerializableDictionaryPair dictPair in leaf.sharedVarDictPairs)
        {
            sharedVarDict.Add(dictPair.pair.Key, dictPair.pair.Value);
        }
        leaf.Init(sharedVarDict);
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

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TestNodeTree : MonoBehaviour
{
    // Use this for initialization
    private BehaviorComponent tree;
    [SerializeField]
    private Object treeReference;

    private Player playerReference;
    private Hostile hostileReference;

    void InitializeLeaf(BehaviorLeaf leaf)
    {
        leaf.Init(gameObject);
    }

    void InitializeTree(ref BehaviorComponent node)
    {
        EditorUtility.SetDirty(node);
        if (node.GetType().IsSubclassOf(typeof(BehaviorComposite)))
        {
            BehaviorComposite composite = (BehaviorComposite)node;

            for(int i = 0; i < composite.childBehaviors.Length; i++)
            {
                InitializeTree(ref composite.childBehaviors[i]);
            }
        }
        else if (node.GetType().IsSubclassOf(typeof(BehaviorLeaf)))
        {
            InitializeLeaf((BehaviorLeaf)node);
        }
    }

    void Start()
    {
        tree = AssetDatabase.LoadAssetAtPath<BehaviorComponent>(AssetDatabase.GetAssetPath(treeReference.GetInstanceID()));
        playerReference = Player.instance;
        hostileReference = GetComponent<Hostile>();
        InitializeTree(ref tree);
    }

    // Update is called once per frame
    void Update()
    {
        tree.Behave();
    }
}

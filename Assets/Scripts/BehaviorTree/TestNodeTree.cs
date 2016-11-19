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

    private List<ObjectReference> objectRefs = new List<ObjectReference>();

    void InitializeLeaf(ref BehaviorComponent leaf)
    {
        if(leaf.GetType() == typeof(ActionMoveTowardsPlayer))
        {
            ((ActionMoveTowardsPlayer)leaf).Init(objectRefs);
        }
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
            InitializeLeaf(ref node);
        }
    }

    void Start()
    {
        tree = AssetDatabase.LoadAssetAtPath<BehaviorComponent>(AssetDatabase.GetAssetPath(treeReference.GetInstanceID()));
        playerReference = Player.instance;
        hostileReference = GetComponent<Hostile>();
        objectRefs.Add(new ObjectReference(playerReference, "player"));
        objectRefs.Add(new ObjectReference(hostileReference, "hostile"));
        InitializeTree(ref tree);
    }

    // Update is called once per frame
    void Update()
    {
        tree.Behave();
    }
}

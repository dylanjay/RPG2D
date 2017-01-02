using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TestNodeTree : BehaviorTree
{
    [SerializeField]
    private GameObject playerReference;

    public override void Awake()
    {
        base.Awake();
    }
}

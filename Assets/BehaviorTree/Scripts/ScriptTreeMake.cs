using UnityEngine;
using System.Collections;
using Benco.BehaviorTree;

/// <remarks>
/// I don't think this is a useful class anymore.
/// </remarks>
public class ScriptTreeMake : MonoBehaviour
{
    BehaviorComponent tree;

    void Update()
    {
        tree.Behave();
    }
}

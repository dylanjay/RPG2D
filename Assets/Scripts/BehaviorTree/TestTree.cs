using UnityEngine;
using System.Collections;

public class TestTree : MonoBehaviour {

    // Use this for initialization
    BehaviorComponent tree;
    void Start()
    {
        //BehaviorComponent[] children = new BehaviorComponent[] { new BehaviorLeaf(), };
        tree =
            new BehaviorSelector("Selector", new BehaviorComponent[]
            {
                new BehaviorRepeater("Inverter",
                //new BehaviorLeaf("Action: Heavy Attack", GetComponent<Entity>().HeavyAttack),
                new BehaviorLeaf<float>("Condition: Above 110% HP", GetComponent<Entity>().PercentHealthAboveRatio, 1.1f) )
                //new BehaviorLeaf("Action: Basic Attack", GetComponent<Entity>().Attack)
            });
    }

    // Update is called once per frame
    void Update()
    {
        tree.Behave();
    }
}

using UnityEngine;
using System.Collections;

public class ScriptTreeMake : MonoBehaviour {

    // Use this for initialization
    BehaviorComponent tree; 
	void Start ()
    {
        //BehaviorComponent[] children = new BehaviorComponent[] { new BehaviorLeaf(), };
        tree =
            new BehaviorSequence("Sequence", new BehaviorComponent[]
            {
                new BehaviorWhile(
                    "While Above 50% HP",
                    new BehaviorLeaf<float>("Condition: Above 50% HP", GetComponent<Entity>().PercentHealthAboveRatio, .5f),
                    new BehaviorLeaf("Action: Basic Attack", GetComponent<Entity>().Attack)
                ),

                new BehaviorLeaf("Action: Heavy Attack", GetComponent<Entity>().HeavyAttack)

            });
	}
	
	// Update is called once per frame
	void Update () {
        tree.Behave();
	}
}

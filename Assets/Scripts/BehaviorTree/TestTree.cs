using UnityEngine;
using System.Collections;

public class TestTree : MonoBehaviour {

    // Use this for initialization
    BehaviorComponent tree;
    Player player;
    void Start()
    {
        player = Player.instance;
        //BehaviorComponent[] children = new BehaviorComponent[] { new BehaviorLeaf(), };
        tree =
            new BehaviorSelector("Selector", new BehaviorComponent[]
            {
                new BehaviorSequence("Sequence", new BehaviorComponent[]
                {
                    new BehaviorLeaf<float>("Condition: In Attack Range", GetComponent<Hostile>().InAttackRange, 2.0f),
                    new BehaviorWait("Wait to aggress to player", new ActionPounce("Pounce", player, GetComponent<Hostile>()), 2.0f)
                }),

                new BehaviorSequence("Sequence", new BehaviorComponent[]
                {
                    new BehaviorLeaf<float>("Condition: In Alert Distance", GetComponent<Hostile>().IsAlert, 8.0f),
                    new ActionMoveTowardsPlayer("Alert", player, GetComponent<Hostile>())
                }),

                new ActionPatrol("Patrol", transform.position, transform.position + new Vector3(0, 5, 0), GetComponent<Hostile>())
            });
    }

    // Update is called once per frame
    void Update()
    {
        tree.Behave();
    }
}

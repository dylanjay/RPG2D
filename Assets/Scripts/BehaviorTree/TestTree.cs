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
                    new ActionWait("Wait to aggress to player", 2.0f, new ActionPounce("Hostile switfly dashes to player", player, GetComponent<Hostile>()))
                }),

                new BehaviorSequence("Sequence", new BehaviorComponent[]
                {
                    new BehaviorLeaf<float>("Condition: In Alert Distance", GetComponent<Hostile>().IsAlert, 8.0f),
                    new ActionMoveTowardsPlayer("Hostile aggresses to player", player, GetComponent<Hostile>())
                }),

                new ActionPatrol("Initial Patrol", transform.position, transform.position + new Vector3(0, 5, 0), GetComponent<Hostile>())
            });
    }

    // Update is called once per frame
    void Update()
    {
        tree.Behave();
    }
}

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
                    new BehaviorLeaf<bool>("Condition: Not in Knockback State", GetComponent<Hostile>().IsKnockback, false),
                    new BehaviorLeaf<float>("Condition: In Alert Distance", GetComponent<Hostile>().Alert, 5.0f),
                    new BehaviorLeaf<Transform>("Action: Move Towards Player", GetComponent<Entity>().MoveTowards, player.transform)
                }),

                new BehaviorSequence("Sequence", new BehaviorComponent[]
                {
                    new BehaviorLeaf<bool>("Condition: In Knockback State", GetComponent<Hostile>().IsKnockback, true),
                    new BehaviorLeaf("Action: Move Away From Player", GetComponent<Hostile>().Knockback)
                })
            });
    }

    // Update is called once per frame
    void Update()
    {
        tree.Behave();
    }
}

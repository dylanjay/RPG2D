using UnityEngine;
using System.Collections;

public class BossTestTree : MonoBehaviour
{

    // Use this for initialization
    BehaviorComponent tree;
    Player player;

    bool inAttackRange = false;
    bool alerted = false;

    private BehaviorState TreeDebug(BehaviorState returnType)
    {
        Debug.Log(returnType);
        return returnType;
    }

    void Start()
    {
        player = Player.instance;
        //BehaviorComponent[] children = new BehaviorComponent[] { new BehaviorLeaf(), };
        /*tree =
            new BehaviorSelector("Selector", new BehaviorComponent[]
            {
                new BehaviorSequence("Sequence", new BehaviorComponent[]
                {
                    //new ActionFunc<float>("Condition: In Attack Range", InAttackRange, 4.0f),
                   // new BehaviorWait("Wait to aggress to player", new ActionPounce("Pounce", player, GetComponent<Hostile>()), 2.0f)
                    new BehaviorMemSequence("Sequence", new BehaviorComponent[]
                    {
                        //new ActionWait("Idle", 2.0f, GetComponent<Hostile>()),
                        //new ActionPounce("Pounce", player, GetComponent<Hostile>())
                    })
                }),

                new BehaviorSequence("Sequence", new BehaviorComponent[]
                {
                    //new ActionFunc<float>("Condition: In Alert Distance", IsAlert, 8.0f),
                    //new ActionMoveTowardsPlayer("Alert", player, GetComponent<Hostile>())
                }),

                //new ActionPatrol("Patrol", transform.position, transform.position + new Vector3(0, 5, 0), GetComponent<Hostile>())
            });*/
    }

    BehaviorState IsAlert(float distance)
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= distance)
        {
            alerted = true;
            return BehaviorState.Success;
        }

        else
        {
            alerted = false;
            return BehaviorState.Failure;
        }
    }

    BehaviorState InAttackRange(float distance)
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= distance)
        {
            inAttackRange = true;
            return BehaviorState.Success;
        }

        else
        {
            inAttackRange = false;
            return BehaviorState.Failure;
        }
    }

    // Update is called once per frame
    void Update()
    {
        tree.Behave();
    }
}

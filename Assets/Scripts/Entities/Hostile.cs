using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    public Animator anim;

    public int lvl = 1;
    public int expGiven = 1;
    Player player;

    List<CastableAbility> abilities;

    float healthBarDisplay = 0.0f;
    float healthBarDisplayMax = 0.0f;
    GameObject healthBarFill;

    GameObject healthBar;

    HealthBarManager healthBarManager;

    public bool hitPlayer = false;
    public bool inAttackRange = false;
    public bool alerted = false;

    public Dictionary<string, int> AnimParamIDs = new Dictionary<string, int>();

    void Awake()
    {
        
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        player = Player.instance;
        healthBarManager = HealthBarManager.instance;
        healthBar = healthBarManager.Create();
        health = new MaxableStat(10, 0.1f);
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        healthBar.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y + GetComponent<Renderer>().bounds.extents.y + 0.2f, pos.z - 1);
    }


    public BehaviorState IsAlert(float distance)
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

    public BehaviorState InAttackRange(float distance)
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

    //TEMPORARY REMOVE LATER
    public void takeDamage(int damage)
    {
        health.value -= 5;
        healthBarManager.UpdateHealthBar(healthBar, health);

        if (health.value <= 0)
        {
            OnDeath();
        }
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Sword")
        {
            health.value -= 5;
            healthBarManager.UpdateHealthBar(healthBar, health);
        }

        if (other.gameObject.name == "Player")
        {
            hitPlayer = true;
        }

        if (health.value <= 0)
        {
            OnDeath();
        }
    }

    protected override void OnDeath()
    {
        if (health.value <= 0)
        {
            Destroy(healthBar);
            Destroy(this.gameObject);

            Player.instance.SetExp(this.expGiven);

            //Debug.Log("Current expGiven :" + hostile.expGiven);
            Debug.Log("Current curLvl :" + Player.instance.curLvl);
            Debug.Log("Current curexp :" + Player.instance.curExp);
        }
    }

}

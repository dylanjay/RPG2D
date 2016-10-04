﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    public bool isBoss;

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

    ItemDatabase itemDatabase;

    public bool hitPlayer = false;

    public class AnimParams
    {
        public static readonly int Direction = Animator.StringToHash("Direction");
        public static readonly int Moving = Animator.StringToHash("Moving");
        public static readonly int SpeedX = Animator.StringToHash("SpeedX");
        public static readonly int SpeedY = Animator.StringToHash("SpeedY");
        public static readonly int Swing = Animator.StringToHash("Swing");
        public static readonly int Alert = Animator.StringToHash("Alert");
        public static readonly int Patrol = Animator.StringToHash("Patrol");
        public static readonly int Pounce = Animator.StringToHash("Pounce");
        public static readonly int Idle = Animator.StringToHash("Idle");
    }

    public BehaviorState SetTrigger(int animParam)
    {
        anim.SetTrigger(animParam);
        return BehaviorState.Success;
    }

    void Awake()
    {
        
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        player = Player.instance;
        itemDatabase = ItemDatabase.instance;
        healthBarManager = HealthBarManager.instance;
        if (!isBoss)
        {
            healthBar = healthBarManager.Create(isBoss);
        }
        else
        {
            healthBar = GameObject.Find("Boss Health Bar");
        }
        health = new MaxableStat(10, 0.1f);
    }

    void LateUpdate()
    {
        if (!isBoss)
        {
            Vector3 pos = transform.position;
            healthBar.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y + GetComponent<Renderer>().bounds.extents.y + 0.2f, pos.z - 1);
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

    void DropItem(Item item)
    {
        GameObject itemInstance = Instantiate(Resources.Load("Prefabs/Item", typeof(GameObject)), transform.position, transform.rotation) as GameObject;
        itemInstance.GetComponent<ItemComponent>().reset(item.id);
        itemInstance.GetComponent<ItemComponent>().setStack(1);
    }

    protected override void OnDeath()
    {
        if (health.value <= 0)
        {
            DropItem(itemDatabase.GetRandomItem(tier));

            Destroy(healthBar);
            Destroy(this.gameObject);

            Player.instance.SetExp(this.expGiven);

            //Debug.Log("Current expGiven :" + hostile.expGiven);
            Debug.Log("Current curLvl :" + Player.instance.curLvl);
            Debug.Log("Current curexp :" + Player.instance.curExp);
        }
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    public bool isBoss;
    public float dropRate = 0.0f;

    [HideInInspector]
    public Animator anim;

    public int lvl = 1;
    public int expGiven = 1;

    List<CastableAbility> abilities;
    
    GameObject healthBar;

    HealthBarManager healthBarManager;

    //CR: We should not have a reference in each Hostile object when ItemDatabase.instance already exists.
    ItemDatabase itemDatabase;

    [HideInInspector]
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
        itemDatabase = ItemDatabase.instance;
        healthBarManager = HealthBarManager.instance;

        healthBar = healthBarManager.RequestHealthBar(isBoss);

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
    //CR: this function seems to be adequate enough to not be temporary.
    public void TakeDamage(int damage)
    {
        health.value -= damage;
        healthBarManager.UpdateHealthBar(healthBar, health);

        if (health.value <= 0)
        {
            Death();
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
            Death();
        }
    }

    void DropItem(Item item)
    {
        GameObject itemInstance = Instantiate(Resources.Load("Prefabs/Item", typeof(GameObject)), transform.position, transform.rotation) as GameObject;
        itemInstance.GetComponent<ItemComponent>().Reset(item.id);
        itemInstance.GetComponent<ItemComponent>().SetStack(1);
    }

    protected override void Death()
    {
        if (health.value <= 0)
        {
            if (Random.value <= dropRate)
            {
                DropItem(itemDatabase.GetRandomItem(tier));
            }

            if (isBoss)
            {
                healthBar.SetActive(false);
            }
            else
            {
                Destroy(healthBar);
            }
            Destroy(gameObject);

            Player.instance.SetExp(expGiven);

            //Debug.Log("Current expGiven :" + hostile.expGiven);
            Debug.Log("Current curLvl :" + Player.instance.curLvl);
            Debug.Log("Current curexp :" + Player.instance.curExp);
        }
    }

}

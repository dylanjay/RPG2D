using UnityEngine;
using System.Collections.Generic;
using Benco.BehaviorTree;

public class Hostile : Entity
{

    [SerializeField]
    bool isBoss;

    [SerializeField]
    float dropRate = 0.0f;

    [HideInInspector]
    public Animator anim;

    int lvl = 1;
    int expGiven = 1;

    List<CastableAbility> abilities;

    GameObject healthBar;

    HealthBarManager healthBarManager;

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

    void Start()
    {
        anim = GetComponent<Animator>();
        healthBarManager = HealthBarManager.instance;

        if (isBoss) { healthBar = healthBarManager.GetBossHealthBar(); }
        else { healthBar = healthBarManager.RequestHealthBar(); }

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
                DropItem(ItemDatabase.instance.GetRandomItem(tier));
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

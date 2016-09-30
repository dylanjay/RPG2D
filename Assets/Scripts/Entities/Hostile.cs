using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    public int lvl = 1;
    public int expGiven = 1;
    Player player;

    public bool knockback = false;
    float knockbackTimer = 2.0f;

    List<CastableAbility> abilities;

    float healthBarDisplay = 0.0f;
    float healthBarDisplayMax = 0.0f;
    GameObject healthBarFill;

    GameObject healthBar;

    HealthBarManager healthBarManager;

    void Awake()
    {
        
    }

    void Start()
    {
        player = Player.instance;
        healthBarManager = HealthBarManager.instance;
        healthBar = healthBarManager.Create();
        healthBarFill = healthBar.transform.FindChild("Fill").gameObject;
        healthBarDisplayMax = healthBarFill.GetComponent<RectTransform>().localScale.x;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        healthBar.GetComponent<RectTransform>().position = new Vector3(pos.x, pos.y + GetComponent<MeshRenderer>().bounds.extents.y + 0.2f, pos.z - 1);
        healthBarDisplay = health.value / health.max;
        Vector3 barScale = healthBarFill.GetComponent<RectTransform>().localScale;
        healthBarFill.GetComponent<RectTransform>().localScale = new Vector3(healthBarDisplayMax * healthBarDisplay, barScale.y, barScale.z);
    }

    public BehaviorState IsKnockback(bool state)
    {
        return (state == knockback ? BehaviorState.Success : BehaviorState.Failure);
    }

    public BehaviorState Knockback()
    {
        MoveAwayFrom(player.transform);
        knockbackTimer -= Time.deltaTime;

        if (knockbackTimer <= 0)
        {
            knockback = false;
            knockbackTimer = 2.0f;
        }
        return BehaviorState.Success;
    }

    public BehaviorState Alert(float distance)
    {
        /*if(Vector2.Distance(transform.position, player.transform.position) <= distance && !knockback)
        {
            MoveTowards(player.transform);  
        }*/

        return (Vector2.Distance(transform.position, player.transform.position) <= distance ? BehaviorState.Success : BehaviorState.Failure);
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.name == "Sword")
            health.value -= 5;

        if (other.gameObject.name == "Player")
            knockback = true;

        if (health.value <= 0)
        {
            OnDeath();
        }
    }

    protected override void OnDeath()
    {
        if (health.value <= 0)
        {
            Destroy(this.gameObject);

            Player.instance.SetExp(this.expGiven);

            //Debug.Log("Current expGiven :" + hostile.expGiven);
            Debug.Log("Current curLvl :" + Player.instance.curLvl);
            Debug.Log("Current curexp :" + Player.instance.curExp);
        }
    }

}

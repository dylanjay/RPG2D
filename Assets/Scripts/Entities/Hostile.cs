using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    public int lvl = 1;
    public int expGiven = 1;
    Player player;

    public bool knockback = false;
    float knockbackTimer = 2.0f;

    void Awake()
    {
        
    }

    void Start()
    {
        player = Player.instance;
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

    protected override void OnTriggerEnter2D(Collider2D other)
    {

    }

    protected override void OnTriggerExit2D(Collider2D other)
    {

    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.name == "Sword")
            health -= 5;

        if (other.gameObject.name == "Player")
            knockback = true;

        if (health <= 0)
        {
            OnDeath();
        }
    }

    protected override void OnCollisionExit2D(Collision2D other)
    {

    }

    protected override void OnDeath()
    {
        if (health <= 0)
        {
            Destroy(this.gameObject);

            Player.instance.SetExp(this.expGiven);

            //Debug.Log("Current expGiven :" + hostile.expGiven);
            Debug.Log("Current curLvl :" + Player.instance.curLvl);
            Debug.Log("Current curexp :" + Player.instance.curExp);
        }
    }

}

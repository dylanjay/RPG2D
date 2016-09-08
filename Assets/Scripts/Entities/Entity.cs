﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour {

    //Stats
    public string entityName = "Mark";
    public string type = "Hostile";//Rude.
    public int health = 10;
    public int defence = 5;
    public int attack = 5;
    public int magic = 5;
    public int mana = 5;

    void Start()
    {

    }

    protected virtual void OnDeath()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {

    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("Current curLvl :" + player.curLvl);
        //Debug.Log("Current curexp :" + player.curExp);
        health -= 5;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {

    }

    public BehaviorState PercentHealthAboveRatio(float ratio)
    {
        return (1 > ratio ? BehaviorState.Success : BehaviorState.Failure);
    }

    public BehaviorState Attack()
    {
        Debug.Log("Regular Attack");
        return BehaviorState.Running;
    }
    public BehaviorState HeavyAttack()
    {
        Debug.Log("Heavy Attack");
        return BehaviorState.Running;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    public int lvl = 1;
    public int expGiven = 1;

    void Awake()
    {
        
    }

    void Start()
    {
        
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {

    }

    protected override void OnTriggerExit2D(Collider2D other)
    {

    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        health -= 5;
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

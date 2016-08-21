using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HostileComponent : MonoBehaviour {

    public Hostile hostile;

    public int hostileID = 1;

    Player player;
    public int health = 0;

    void Start()
    {
        hostile = EntityDatabase.instance.GetEntityByID(hostileID) as Hostile;
        this.health = hostile.stats["Health"];
        player = PlayerControl.instance.player;
    }

    public void onDeath()
    {
        if (health <= 0)
        {
            Destroy(this.gameObject);
            
            player.setExp(hostile.expGiven);

            //Debug.Log("Current expGiven :" + hostile.expGiven);
            Debug.Log("Current curLvl :" + player.curLvl);
            Debug.Log("Current curexp :" + player.curExp);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

    }

    void OnTriggerExit2D(Collider2D other)
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("Current curLvl :" + player.curLvl);
        //Debug.Log("Current curexp :" + player.curExp);
        health -= 5;
        if (health <= 0)
        {
            onDeath();
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {

    }


}

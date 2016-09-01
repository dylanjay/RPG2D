using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour {

    //Stats
    public int ID = 1;
    public string entityName = "Mark";
    public string type = "Hostile";
    public int health = 10;
    public int defence = 5;
    public int attack = 5;
    public int magic = 5;
    public int mana = 5;

    public Hostile hostile;

    Player player;

    void Start()
    {
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

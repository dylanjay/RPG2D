using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity
{
    public static Player instance;

    //Level
    public int curLvl = 1;
    public const int maxLvl = 6;

    //Experience
    public int curExp = 0;
    public int[] expToLvl = new int[maxLvl] {0, 1, 2, 4, 8, 16 };

    public int combo = 0;

    void Awake()
    {
        instance = this;
        this.health = 50;
    }

    void Start()
    {
        
    }

    public void IncrementCombo()
    {
        combo++;
        this.attack++;
        Debug.Log("Current combo multiplier: " + combo);
    }

    public void ResetCombo()
    {
        this.attack -= combo;
        combo = 0;
        Debug.Log("Combo Reset");
    }

    public void SetExp(int expAmount)
    {
        if (curLvl != maxLvl)
        {
            curExp += expAmount;
            for (int i = curLvl; i < maxLvl; i++)
            {
                if (curExp >= expToLvl[curLvl])
                {
                    curExp -= expToLvl[curLvl];
                    curLvl++; //function for lvling up?
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void UpdateStats(Dictionary<string, Item> equipmentSlots)
    {
        foreach(Item item in equipmentSlots.Values)
        {
            if (item.id != -1 && item != null)
            {
                for (int i = 0; i < item.stats.Count; i++)
                {
                    Stat stat = item.stats[i];

                    switch (stat.name)
                    {
                        case "Power":
                            this.attack += stat.value; //Change names accordingly
                            break;
                        case "Defence":
                            this.defence += stat.value;
                            break;
                        case "Vitality":
                            this.health += stat.value;
                            break;
                    }
                }
            }
        }
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
            Debug.Log("Oh no you died!");
        }
    }

}

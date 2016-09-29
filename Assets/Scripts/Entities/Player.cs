using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity
{
    public static Player instance;

    //Level
    public int curLvl = 1;
    public const int maxLvl = 6;

    //Experience
    public int curExp = 0;
    public int[] expToLvl = new int[maxLvl] {0, 1, 2, 4, 8, 16 };

    public int combo = 0;

    float healthBarDisplay = 0.0f;
    float healthBarDisplayMax = 0.0f;
    public GameObject healthBarFill;

    void Awake()
    {
        instance = this;
        health.value = 50;
    }

    void Start()
    {
        healthBarDisplayMax = healthBarFill.GetComponent<RectTransform>().localScale.x;
    }

    void Update()
    {
        healthBarDisplay = health.value / health.max;
        Vector3 barScale = healthBarFill.GetComponent<RectTransform>().localScale;
        healthBarFill.GetComponent<RectTransform>().localScale = new Vector3(healthBarDisplayMax * healthBarDisplay, barScale.y, barScale.z);
    }

    public void IncrementCombo()
    {
        combo++;
        attack.value++;
        Debug.Log("Current combo multiplier: " + combo);
    }

    public void ResetCombo()
    {
        attack.value -= combo;
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
                            attack.value += stat.value; //Change names accordingly
                            break;
                        case "Defence":
                            defence.value += stat.value;
                            break;
                        case "Vitality":
                            health.value += stat.value;
                            break;
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Hostile>())
        {
            health.value -= 5;
        }

        if (health.value <= 0)
        {
            OnDeath();
        }
    }
    
    protected override void OnDeath()
    {
        if (health.value <= 0)
        {
            //Destroy(gameObject);
            Update();
            gameObject.SetActive(false);
            Debug.Log("Oh no you died!");
        }
    }

}

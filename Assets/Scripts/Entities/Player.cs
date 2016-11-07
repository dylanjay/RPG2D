using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class Player : Entity
{
    public static Player instance { get; private set; }

    PlayerStats stats;
    SkillTree skillTree;

    //Level
    public int curLvl = 1;
    public const int maxLvl = 6;

    //Experience
    public int curExp = 0;
    public int[] expToLvl = new int[maxLvl] {0, 1, 2, 4, 8, 16 };

    HealthBarManager healthBarManager;
    GameObject healthBar;

    void Awake()
    {
        instance = this;
        health.value = 50;
    }

    void Start()
    {
        healthBarManager = HealthBarManager.instance;
        healthBar = healthBarManager.GetPlayerHealthBar();
        stats = PlayerStats.instance;
        skillTree = SkillTree.instance;
    }

    void LevelUp()
    {
        curLvl++;
        stats.LevelUp();
        skillTree.LevelUp();
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
                    LevelUp();
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void UpdateStats(Dictionary<Inventory.EquipmentType, Item> equipmentSlots)
    {
        foreach(Item item in equipmentSlots.Values)
        {
            if (item.id != -1 && item != null)
            {
                for (int i = 0; i < item.stats.Count; i++)
                {
                    ItemStat stat = item.stats[i];

                    switch (stat.name)
                    {
                        case "Power":
                            attack.value += stat.value;
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

    public void SaveStats()
    {

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Hostile>())
        {
            health.value -= 5;
            healthBarManager.UpdateHealthBar(healthBar, health);
        }

        if (health.value <= 0)
        {
            Death();
        }
    }
    
    protected override void Death()
    {
        if (health.value <= 0)
        {
            //Destroy(gameObject);
            healthBarManager.UpdateHealthBar(healthBar, health);
            gameObject.SetActive(false);
            Debug.Log("Oh no you died!");
        }
    }

}

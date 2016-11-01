using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance { get; private set; }

    public int strength = 10;
    public int endurance = 10;
    public int charisma = 10;
    public int intelligence = 10;
    public int agility = 10;

    Player player;

    public int availablePoints = 0;
    public int pointsPerLvl;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player = Player.instance;
    }

    public void LevelUp()
    {
        availablePoints += pointsPerLvl;
    }

    public int GetStat(string statName)
    {
        switch (statName)
        {
            case "Strength":
                return strength;

            case "Endurance":
                return endurance;

            case "Charisma":
                return charisma;

            case "Intelligence":
                return intelligence;

            case "Agility":
                return agility;

            default:
                return -1;
        }
    }

    public void SetStat(string statName, int value)
    {
        switch(statName)
        {
            case "Strength":
                strength = value;
                break;

            case "Endurance":
                endurance = value;
                break;

            case "Charisma":
                charisma = value;
                break;

            case "Intelligence":
                intelligence = value;
                break;

            case "Agility":
                agility = value;
                break;
        }
    }
}

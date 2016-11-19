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

    public int GetStat(SkillTree.SkillStat stat)
    {
        switch (stat)
        {
            case SkillTree.SkillStat.Strength:
                return strength;
            case SkillTree.SkillStat.Endurance:
                return endurance;
            case SkillTree.SkillStat.Charisma:
                return charisma;
            case SkillTree.SkillStat.Intelligence:
                return intelligence;
            case SkillTree.SkillStat.Agility:
                return agility;
            default:
                Debug.LogError("Incorrect stat on GetStat");
                return -1;
        }
    }

    public void SetStat(SkillTree.SkillStat stat, int value)
    {
        switch (stat)
        {
            case SkillTree.SkillStat.Strength:
                strength = value;
                break;
            case SkillTree.SkillStat.Endurance:
                endurance = value;
                break;
            case SkillTree.SkillStat.Charisma:
                charisma = value;
                break;
            case SkillTree.SkillStat.Intelligence:
                intelligence = value;
                break;
            case SkillTree.SkillStat.Agility:
                agility = value;
                break;
            default:
                Debug.LogError("Incorrect stat on SetStat");
                break;
        }
    }
}

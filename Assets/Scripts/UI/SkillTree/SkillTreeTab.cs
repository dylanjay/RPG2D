using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SkillTreeTab : MonoBehaviour, IPointerDownHandler
{
    PlayerStats playerStats;
    SkillTree skillTree;

    public string statName;
    public int points;
    public SkillTree.SkillTab tab;

    void Start ()
    {
        playerStats = PlayerStats.instance;
        skillTree = SkillTree.instance;
        statName = name.Substring(0, name.Length - 4);
        points = playerStats.GetStat(statName);
        switch(statName)
        {
            case "Strength":
                tab = SkillTree.SkillTab.Strength;
                break;

            case "Endurance":
                tab = SkillTree.SkillTab.Endurance;
                break;

            case "Charisma":
                tab = SkillTree.SkillTab.Charisma;
                break;

            case "Intelligence":
                tab = SkillTree.SkillTab.Intelligence;
                break;

            case "Agility":
                tab = SkillTree.SkillTab.Agility;
                break;
        }
	}

    

    public void OnPointerDown(PointerEventData eventData)
    {
        skillTree.SwitchTrees(tab);
    }
}

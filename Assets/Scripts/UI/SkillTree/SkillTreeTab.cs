using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SkillTreeTab : MonoBehaviour, IPointerDownHandler
{
    PlayerStats playerStats;
    SkillTree skillTree;

    [SerializeField]
    Transform tree;

    public string statName;
    public int points;
    public SkillTree.SkillTab tab;

    void Start()
    {
        playerStats = PlayerStats.instance;
        skillTree = SkillTree.instance;
        //TODO change to enum
        statName = name.Substring(0, name.Length - 4);
        points = PlayerStats.instance.GetStat(statName);

        InitializTreeeNodes();

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

    void InitializTreeeNodes()
    {
        for (int i = 0; i < tree.childCount; i++)
        {
            for (int j = 0; j < tree.GetChild(i).childCount; j++)
            {
                SkillTreeNode node = tree.GetChild(i).GetChild(j).GetComponent<SkillTreeNode>();
                if (points > tree.GetChild(i).GetChild(j).GetComponent<SkillTreeNode>().pointRequirement)
                {
                    node.Activate();
                }

                else
                {
                    node.Deactivate();
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        skillTree.SwitchTrees(tab);
    }
}

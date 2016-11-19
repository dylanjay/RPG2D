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

    public int points;
    [SerializeField]
    SkillTree.SkillStat _skillStat;
    public SkillTree.SkillStat skillStat { get { return _skillStat; } private set { _skillStat = value; } }

    void Start()
    {
        playerStats = PlayerStats.instance;
        skillTree = SkillTree.instance;
        points = PlayerStats.instance.GetStat(skillStat);

        InitializeTreeNodes();
	}

    void InitializeTreeNodes()
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
        skillTree.SwitchTrees(skillStat);
    }
}

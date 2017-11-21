using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System;

public class SkillTreeConfirmationButton : MonoBehaviour, IPointerDownHandler
{
    PlayerStats playerStats;

    Transform treeTabs;

    bool confirmer = false;

    void Start ()
    {
        playerStats = PlayerStats.instance;

        treeTabs = transform.parent.parent.Find("Tree Tabs");

        if(name == "Confirm Button")
        {
            confirmer = true;
        }
    }

    void ConfirmPoints()
    {   
        int pointsUsed = 0;
        for(int i = 0; i < treeTabs.childCount; i++)
        {
            SkillTreeTab tab = treeTabs.GetChild(i).GetComponent<SkillTreeTab>();
            string pointsString = treeTabs.GetChild(i).Find("Tree Points").GetComponent<Text>().text;
            int points = int.Parse(pointsString);
            pointsUsed += points - tab.points;
            tab.points = points;
            playerStats.SetStat(tab.skillStat, tab.points);
            treeTabs.GetChild(i).Find("Decrement Button").gameObject.SetActive(false);

            for(int j = 0; j < treeTabs.GetChild(i).childCount; j++)
            {
                for(int k = 0; k < treeTabs.GetChild(i).GetChild(j).childCount; k++)
                {
                    SkillTreeNode node = treeTabs.GetChild(i).GetChild(j).GetChild(k).GetComponent<SkillTreeNode>();
                    if(tab.points >= node.pointRequirement)
                    {
                        node.Activate();
                    }
                    //TODO check for skill requirements
                }
            }
        }

        if(playerStats.availablePoints == 0)
        {
            for(int i = 0; i < treeTabs.childCount; i++)
            {
                treeTabs.GetChild(i).Find("Increment Button").gameObject.SetActive(false);
            }

            transform.parent.gameObject.SetActive(false);
        }
    }

    void RevertPoints()
    {
        for(int i = 0; i < treeTabs.childCount; i++)
        {
            SkillTreeTab tab = treeTabs.GetChild(i).GetComponent<SkillTreeTab>();
            string pointsString = treeTabs.GetChild(i).Find("Tree Points").GetComponent<Text>().text;
            int inflatedPoints = int.Parse(pointsString);
            int pointsReverted = inflatedPoints - tab.points;
            playerStats.availablePoints += pointsReverted;
            treeTabs.GetChild(i).Find("Tree Points").GetComponent<Text>().text = tab.points.ToString();
            treeTabs.GetChild(i).Find("Decrement Button").gameObject.SetActive(false);
        }

        if(playerStats.availablePoints > 0)
        {
            for (int i = 0; i < treeTabs.childCount; i++)
            {
                treeTabs.GetChild(i).Find("Increment Button").gameObject.SetActive(true);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(confirmer)
        {
            ConfirmPoints();
        }

        else
        {
            RevertPoints();
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

public class SkillTreePointButton : MonoBehaviour, IPointerDownHandler
{
    PlayerStats playerStats;

    bool incrementer = false;

    public int pointMax = 30;

    [SerializeField]
    public Text pointsText;

    string statName;


    void Start()
    {
        playerStats = PlayerStats.instance;

        statName = transform.parent.GetComponent<SkillTreeTab>().statName;

        if (name == "Increment Button")
        {
            incrementer = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SkillTreeTab tab = transform.parent.GetComponent<SkillTreeTab>();
        int points = int.Parse(pointsText.text);
        if (incrementer && points < pointMax && playerStats.availablePoints > 0)
        {
            points++;
            playerStats.availablePoints--;
            pointsText.text = points.ToString();

            if(points == tab.points + 1)
            {
                transform.parent.FindChild("Decrement Button").gameObject.SetActive(true);
            }

            if(playerStats.availablePoints == 0)
            {
                gameObject.SetActive(false);
            }
        }

        else if(!incrementer && points > 0)
        {
            if(playerStats.availablePoints == 0)
            {
                transform.parent.FindChild("Increment Button").gameObject.SetActive(true);
            }

            points--;
            playerStats.availablePoints++;
            pointsText.text = points.ToString();

            if (points == tab.points)
            {
                transform.parent.FindChild("Decrement Button").gameObject.SetActive(false);
            }
        }
    }
}

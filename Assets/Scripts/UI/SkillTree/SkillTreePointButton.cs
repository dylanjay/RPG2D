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

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SkillTreeTab tab = transform.parent.GetComponent<SkillTreeTab>();
        int points = tab.points;
        if (incrementer && points < pointMax)
        {
            points++;
            tab.points++;
            playerStats.SetStat(statName, points);
            pointsText.text = points.ToString();
        }

        else if(!incrementer && points > 0)
        {
            points--;
            tab.points--;
            playerStats.SetStat(statName, points);
            pointsText.text = points.ToString();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class SkillTreeNode : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    SkillTree skillTree;
    public Sprite sprite;
    public int slot;
    public GameObject skill;
    public string skillName;

    string statName;
    SkillTree.SkillTab tabName;

    SkillTreeTab parentTab;

    public bool active;
    public float deactivatedColorAlpha = 0.5f;
    public int tier;
    public int pointRequirement;

    //TODO fill skillRequirements list from string list
    public List<string> skillStringRequirements;
    List<Ability> skillRequirements;

    void Start ()
    {
        skillTree = SkillTree.instance;
        skillName = skill.name;
        sprite = skill.GetComponent<UnityEngine.UI.Image>().sprite;
        statName = transform.parent.parent.name.Substring(0, transform.parent.parent.name.Length - 5);
        tier = int.Parse(transform.parent.name.Substring(5));
        parentTab = skillTree.GetTab(statName);
        tabName = parentTab.tab;

        if (parentTab.points > pointRequirement)
        {            
            Activate();
        }

        else
        {
            Deactivate();
        }
    }

    public void Activate()
    {
        active = true;
        transform.GetComponent<Image>().color = Color.white;
    }

    public void Deactivate()
    {
        active = false;
        Color curColor = transform.GetComponent<Image>().color;
        Debug.Log(deactivatedColorAlpha);
        curColor.a = deactivatedColorAlpha;
        transform.GetComponent<Image>().color = curColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (active)
        {
            if (skillTree.slotSelected)
            {
                skillTree.AssignSlot(slot);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (active)
        {
            skillTree.cancelSelection = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (active)
        {
            skillTree.cancelSelection = true;
        }
    }
}

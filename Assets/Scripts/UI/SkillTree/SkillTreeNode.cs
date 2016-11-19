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

    [SerializeField]
    public Ability ability;

    [SerializeField]
    SkillTree.SkillStat skillStat;

    bool active;

    float deactivatedColorAlpha = 0.5f;

    [SerializeField]
    int tier;

    [SerializeField]
    int _pointRequirement;
    public int pointRequirement { get { return _pointRequirement; } private set { _pointRequirement = value; } }
    
    //TODO fill skillRequirements list from string list
    public List<string> skillStringRequirements;
    List<Ability> skillRequirements;

    void Start ()
    {
        skillTree = SkillTree.instance;
        skillName = skill.name;
        sprite = skill.GetComponent<Image>().sprite;
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
        curColor.a = deactivatedColorAlpha;
        transform.GetComponent<Image>().color = curColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(skillTree.swapSlot)
        {
            skillTree.CancelSelection();
        }

        if (active)
        {
            if (skillTree.skillSelected && skillTree.selectedSkillSlot == slot)
            {
                skillTree.CancelSelection();
            }

            
            else if(skillTree.selectedSkillSlot != slot)
            {
                if(!skillTree.skillSelected)
                {
                    skillTree.skillSelected = true;
                }
                skillTree.selectedSkillSlot = slot;
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

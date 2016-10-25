using UnityEngine;
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

    void Start ()
    {
        skillTree = SkillTree.instance;
        skillName = skill.name;
        sprite = skill.GetComponent<UnityEngine.UI.Image>().sprite;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(skillTree.slotSelected)
        {
            skillTree.AssignSlot(slot);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        skillTree.cancelSelection = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillTree.cancelSelection = true;
    }
}

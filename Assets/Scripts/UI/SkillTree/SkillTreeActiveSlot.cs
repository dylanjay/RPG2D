using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class SkillTreeActiveSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    SkillTree skillTree;
    public int slot;
    public bool slotted = false;
    public GameObject skill;
    public Sprite sprite;

	void Start ()
    {
        skillTree = SkillTree.instance;
	}

    public void SlotSkill(string skillName)
    {
        slotted = true;
        if(!skill.activeSelf)
        {
            skill.SetActive(true);
        }
        skill.name = skillName;
        skill.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
    }

    public void UnSlotSkill()
    {
        slotted = false;
        skill.name = "Empty";
        skill.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        skillTree.selectedSlotID = slot;
        skillTree.slotSelected = true;
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

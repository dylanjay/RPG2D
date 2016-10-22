using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SkillTree : MonoBehaviour
{

    AbilityManager abilityManager;
    Transform skillTree;
    Transform activeSkills;
    Player player;
    GameObject playerGO;
    bool slotSelected = false;
    int slotID;

    void Start()
    {
        //TODO initialize skill tree with prefab (based on class?)
        abilityManager = transform.parent.GetComponent<AbilityManager>();
        skillTree = GameObject.Find("Skill Tree Panel").transform;
        activeSkills = skillTree.FindChild("Active Skill Slots");
        skillTree = skillTree.FindChild("Skill Tree Slots");
        player = GetComponentInParent<Player>();
        playerGO = GameObject.Find("Player");
        //TODO enable abilities from save point
    }

    void Update()
    {
    }

    public void selectSlot()
    {
        Transform initial = EventSystem.current.currentSelectedGameObject.transform;
        slotID = int.Parse(initial.name.Substring(5));
        slotSelected = true;
    }

    public void assignSlot()
    {
        Transform initial = EventSystem.current.currentSelectedGameObject.transform;
        string skillName = initial.GetChild(0).name;
        if (slotSelected && !abilityManager.isEquipped(skillName))
        {
            abilityManager.EnableAbility(abilityManager.abilityDict[skillName], playerGO);
            Transform skill = activeSkills.GetChild(slotID).GetChild(0);
            if (!skill.gameObject.activeSelf)
            {
                skill.gameObject.SetActive(true);
            }
            skill.name = skillName;
            skill.GetComponent<UnityEngine.UI.Image>().sprite = initial.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite;
        }
        slotSelected = false;
    }
}

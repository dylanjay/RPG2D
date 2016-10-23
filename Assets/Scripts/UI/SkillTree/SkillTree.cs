using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkillTree : MonoBehaviour
{
    public static SkillTree instance { get { return _instance; } }
    private static SkillTree _instance;

    AbilityManager abilityManager;
    Transform skillTree;
    Transform activeSkills;
    Player player;
    GameObject playerGO;
    bool slotSelected = false;
    int slotID;
    int maxAbilities = 4;
    public List<string> slots = new List<string>();
    public List<string> slotSprites = new List<string>();

    void Start()
    {
        _instance = this;
        //TODO initialize skill tree with prefab (based on class?)
        abilityManager = transform.parent.GetComponent<AbilityManager>();
        skillTree = GameObject.Find("Skill Tree Panel").transform;
        activeSkills = skillTree.FindChild("Active Skill Slots");
        skillTree = skillTree.FindChild("Skill Tree Slots");
        player = GetComponentInParent<Player>();
        playerGO = GameObject.Find("Player");

        for(int i = 0; i < maxAbilities; i++)
        {
            slots.Add("Empty");
            slotSprites.Add("Empty");
        }
    }

    void Update()
    {
    }

    public void SelectSlot()
    {
        Transform initial = EventSystem.current.currentSelectedGameObject.transform;
        slotID = int.Parse(initial.name.Substring(5));
        slotSelected = true;
    }

    public void AssignSlot()
    {
        Transform initial = EventSystem.current.currentSelectedGameObject.transform;
        string skillName = initial.GetChild(0).name;
        if (slotSelected && !abilityManager.isEquipped(skillName))
        {
            Ability ability = abilityManager.abilityDict[skillName];
            if (ability is CastableAbility)
            {
                CastableAbility castableAbility = (CastableAbility)ability;
                castableAbility.keybinding = "Ability" + slotID.ToString();
            }
            abilityManager.EnableAbility(abilityManager.abilityDict[skillName], playerGO);
            Transform skill = activeSkills.GetChild(slotID).GetChild(0);
            if (!skill.gameObject.activeSelf)
            {
                skill.gameObject.SetActive(true);
            }
            skill.name = skillName;
            skill.GetComponent<UnityEngine.UI.Image>().sprite = initial.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite;
            slots[slotID] = skillName;
            slotSprites[slotID] = skill.GetComponent<UnityEngine.UI.Image>().sprite.name;
        }
        slotSelected = false;
    }

    public void LoadSlots(List<string> newSlots, List<string> slotSprites)
    {
        slots = newSlots;
        for(int i = 0; i < activeSkills.childCount; i++)
        {
            Transform skill = activeSkills.GetChild(i).GetChild(0);
            skill.name = newSlots[i];
            Sprite sprite = Resources.Load("Items/" + slotSprites[i], typeof(Sprite)) as Sprite;
            skill.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            if (newSlots[i] == "Empty")
            {
                skill.gameObject.SetActive(false);
            }
            else
            {
                skill.gameObject.SetActive(true);
            }
        }
    }
}

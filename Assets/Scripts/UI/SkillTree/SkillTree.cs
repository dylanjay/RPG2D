using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkillTree : MonoBehaviour
{
    public static SkillTree instance { get; private set; }

    public enum SkillTab { Strength = 0,  Endurance = 1, Charisma = 2, Intelligence = 3, Agility = 4 }

    AbilityManager abilityManager;
    Transform skillTree;
    Transform activeSlotsUI;
    Player player;
    GameObject playerGO;
    int maxAbilities = 4;

    public bool slotSelected = false;
    public int selectedSlotIndex;
    public bool cancelSelection = true;

    SkillTab currentTab;
    int numTabs = 5;
    List<List<SkillTreeNode>> treeNodes = new List<List<SkillTreeNode>>();
    List<SkillTreeActiveSlot> activeSlots = new List<SkillTreeActiveSlot>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //TODO initialize skill tree with prefab (based on class?)
        abilityManager = AbilityManager.instance;
        skillTree = transform.FindChild("Skill Tree Panel").FindChild("Tree Viewer");
        player = Player.instance;
        playerGO = player.gameObject;
        currentTab = SkillTab.Strength;
        skillTree.GetComponent<ScrollRect>().content = skillTree.GetChild((int)currentTab).GetComponent<RectTransform>();
        skillTree.GetChild((int)currentTab).gameObject.SetActive(true);

        for(int i  = 0; i < numTabs; i++)
        {
            treeNodes.Add(new List<SkillTreeNode>());
        }

        for (int i = 0; i < skillTree.childCount; i++)
        {
            int slotNum = 0;
            for (int j = 0; j < skillTree.GetChild(i).childCount; j++)
            {
                for(int k = 0; k < skillTree.GetChild(i).GetChild(j).childCount; k++)
                {
                    SkillTreeNode node = skillTree.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<SkillTreeNode>();
                    node.slot = slotNum;
                    slotNum++;
                    treeNodes[i].Add(node);
                }
            }
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && cancelSelection)
        {
            slotSelected = false;
        }
    }

    public void LevelUp()
    {
        Transform currentTab = skillTree.parent.FindChild("Tree Tabs");
        for (int i = 0; i < currentTab.childCount; i++)
        {
            currentTab.GetChild(i).FindChild("Increment Button").gameObject.SetActive(true);
        }
    }

    public void SwitchTrees(SkillTab newTab)
    {
        if (newTab != currentTab)
        {
            skillTree.GetChild((int)currentTab).gameObject.SetActive(false);
            skillTree.GetChild((int)newTab).gameObject.SetActive(true);
            skillTree.GetComponent<ScrollRect>().content = skillTree.GetChild((int)newTab).GetComponent<RectTransform>();
            currentTab = newTab;
        }
    }

    public void AssignSlot(int slot)
    {
        AbilityBarIcon skillSlot = abilityManager.skillSlots[selectedSlotIndex];
        SkillTreeNode node = treeNodes[(int)currentTab][slot];

        string skillName = node.skillName;
        if (!abilityManager.isEquipped(skillName))
        {
            //activeSlot.sprite = node.sprite;
            //activeSlot.SlotSkill(skillName);
            Ability ability = abilityManager.abilityDict[skillName];
            if (ability is CastableAbility)
            {
                CastableAbility castableAbility = (CastableAbility)ability;
                castableAbility.keybinding = skillSlot.keybinding;
            }
            abilityManager.EnableAbility(abilityManager.abilityDict[skillName], playerGO, selectedSlotIndex);
        }
        slotSelected = false;
    }

    public List<string> SaveSlots(List<string> slotSprites)
    {
        List<string> slots = new List<string>();
        foreach(SkillTreeActiveSlot slot in activeSlots)
        {
            slots.Add(slot.skill.name);
            if (slot.slotted)
            {
                slotSprites.Add(slot.sprite.name);
            }
            else
            {
                slotSprites.Add("Empty");
            }
        }
        return slots;
    }

    public SkillTreeTab GetTab(string statName)
    {
        Transform treeTabs = transform.FindChild("Skill Tree Panel").FindChild("Tree Tabs");
        switch (statName)
        {
            case "Strength":
                return treeTabs.FindChild("Strength Tab").GetComponent<SkillTreeTab>();

            case "Endurance":
                return treeTabs.FindChild("Endurance Tab").GetComponent<SkillTreeTab>();

            case "Charisma":
                return treeTabs.FindChild("Charisma Tab").GetComponent<SkillTreeTab>();

            case "Intelligence":
                return treeTabs.FindChild("Intelligence Tab").GetComponent<SkillTreeTab>();

            case "Agility":
                return treeTabs.FindChild("Agility Tab").GetComponent<SkillTreeTab>();

            default:
                Debug.LogError("Incorrect Stat Name on GetTab");
                return null;
        }
    }

    /*
    //CR: This function has a lot of potential to crash the game. It's actually impressive.
    //If I were to rename the slot, it will cause issues.
    //If I were to add a button that also happens to be selectable, it can try to parse that button as a slot.
    //Also, when calling a function from the UI (Especially when it's only the UI, add a comment above
    //the function so people will know where it gets called from, and that it's not useless.

    //CR: Also, we can completely remove the one of the possibilies of this crashing the game by taking in a
    //transform as a parameter, and passing the slot's transform as the argument.
    //I.E public void SelectedSlot(Transform selectedSlot)
    /// <summary>
    /// Called when the user clicks on a slot in the skill selection UI.
    /// </summary>
    public void SelectSlot()
    {
        //CR: This is a terrible variable name.
        Transform initial = EventSystem.current.currentSelectedGameObject.transform;
        slotID = int.Parse(initial.name.Substring(5));
        slotSelected = true;
    }

    public void AssignSlot()
    {
        //CR: This is a terrible variable name.
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

            //CR: Knowing that our UI is going to change in the future (at least aesthetically), 
            //it's best to make a MonoBehavior for these UI slots rather than accessing a child
            //via an index and setting it's image manually. It also becomes far simpler to understand
            //what
            //      GetComponent<SkillSlot>().skillSlotted != null
            //or even
            //      GetComponent<SkillSlot>().hasSkill
            //means, rather than figuring out what this line of code is supposed to represent
            //(which is a combination of the two lines below):
            //      !activeSkills.GetChild(slotID).GetChild(0).gameObject.activeSelf

            Transform skill = activeSkills.GetChild(slotID).GetChild(0);
            if (!skill.gameObject.activeSelf)
            {
                skill.gameObject.SetActive(true);
            }
            skill.name = skillName;
            //CR: If we only need a namespace once or twice, it's fine to write it out as below. If we need it more than
            //that and we don't have conflicting namespaces, use should a using statement instead.
            skill.GetComponent<UnityEngine.UI.Image>().sprite = initial.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite;
            slots[slotID] = skillName;
            slotSprites[slotID] = skill.GetComponent<UnityEngine.UI.Image>().sprite.name;
        }
        slotSelected = false;
    }*/

    //CR: This function would also become more readable with a MonoBehavior for slots.
    public void LoadSlots(List<string> newSlots, List<string> slotSprites)
    {
        for(int i = 0; i < activeSlots.Count; i++)
        {
            if (newSlots[i] != "Empty")
            {
                Sprite sprite = Resources.Load("Items/" + slotSprites[i], typeof(Sprite)) as Sprite;
                activeSlots[i].sprite = sprite;
                activeSlots[i].SlotSkill(newSlots[i]);
            }
            else
            {
                activeSlots[i].UnSlotSkill();
            }
        }
    }
}

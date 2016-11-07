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

    [SerializeField]
    GameObject confirmationButtons;

    AbilityManager abilityManager;
    Transform skillTree;
    Transform activeSlotsUI;
    Player player;
    GameObject playerGO;
    int maxAbilities = 4;

    [HideInInspector]
    public bool skillSelected = false;
    [HideInInspector]
    public int selectedSlotIndex;
    [HideInInspector]
    public int selectedSkillSlot;
    [HideInInspector]
    public bool cancelSelection = true;
    [HideInInspector]
    public bool swapSlot = false;

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

    public void CancelSelection()
    {
        skillSelected = false;
        selectedSkillSlot = -1;
        selectedSlotIndex = -1;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && cancelSelection)
        {
            CancelSelection();
        }
    }

    public SkillTreeNode GetNode(int slot)
    {
        return treeNodes[(int)currentTab][slot];
    }

    public void LevelUp()
    {
        Transform currentTab = skillTree.parent.FindChild("Tree Tabs");
        for (int i = 0; i < currentTab.childCount; i++)
        {
            currentTab.GetChild(i).FindChild("Increment Button").gameObject.SetActive(true);
        }
        confirmationButtons.SetActive(true);
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
            Ability ability = abilityManager.abilityDict[skillName];
            if (ability is CastableAbility)
            {
                CastableAbility castableAbility = (CastableAbility)ability;
                castableAbility.keybinding = skillSlot.keybinding;
            }
            abilityManager.EnableAbility(abilityManager.abilityDict[skillName], playerGO, selectedSlotIndex);
        }
        skillSelected = false;
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

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkillTree : MonoBehaviour
{
    public static SkillTree instance { get; private set; }

    AbilityManager abilityManager;
    Transform skillTree;
    Transform activeSlotsUI;
    Player player;
    GameObject playerGO;
    int maxAbilities = 4;

    public bool slotSelected = false;
    public int selectedSlotID;
    public bool cancelSelection = true;

    List<SkillTreeNode> treeNodes = new List<SkillTreeNode>();
    List<SkillTreeActiveSlot> activeSlots = new List<SkillTreeActiveSlot>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //TODO initialize skill tree with prefab (based on class?)
        abilityManager = AbilityManager.instance;
        skillTree = transform.FindChild("Skill Tree Panel");
        activeSlotsUI = skillTree.FindChild("Active Skill Slots");
        skillTree = skillTree.FindChild("Skill Tree Slots");
        player = Player.instance;
        playerGO = player.gameObject;

        for (int i = 0; i < activeSlotsUI.childCount; i++)
        {
            SkillTreeActiveSlot activeSlot = activeSlotsUI.transform.GetChild(i).GetComponent<SkillTreeActiveSlot>();
            activeSlot.slot = i;
            activeSlots.Add(activeSlot);
        }

        for (int i = 0; i < skillTree.childCount; i++)
        {
            SkillTreeNode node = skillTree.transform.GetChild(i).GetComponent<SkillTreeNode>();
            node.slot = i;
            treeNodes.Add(node);
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && cancelSelection)
        {
            slotSelected = false;
        }
    }

    public void AssignSlot(int slot)
    {
        SkillTreeActiveSlot activeSlot = activeSlots[selectedSlotID];
        SkillTreeNode node = treeNodes[slot];

        string skillName = node.skillName;
        if (!abilityManager.isEquipped(skillName))
        {
            activeSlot.sprite = node.sprite;
            activeSlot.SlotSkill(skillName);
            Ability ability = abilityManager.abilityDict[skillName];
            if (ability is CastableAbility)
            {
                CastableAbility castableAbility = (CastableAbility)ability;
                castableAbility.keybinding = "Ability" + selectedSlotID.ToString();
            }
            abilityManager.EnableAbility(abilityManager.abilityDict[skillName], playerGO);
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

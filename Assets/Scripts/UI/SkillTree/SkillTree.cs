using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class SkillTree : MonoBehaviour
{
    //CR: The private variable wrapped by the public property should always be above the property, not below.
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

    //CR: Making these variables public is pretty bad practice. The only time you need these variables
    //outside of this object is when you want to save/load them. Unfortunately, making them public also
    //allows other scripts to mutate their contents, like adding and removing elements. It would be far
    //better to return a duplicated list on load/save and prevent access to these lists otherwise than
    //it world be to keep them public and risk bad someone succumbing to laziness and mutating the
    //list without the SkillTree knowing.
    public List<string> slots = new List<string>();
    public List<string> slotSprites = new List<string>();

    void Start()
    {
        //CR: Setting the instance should happen in Awake, not Start. The reason for this is that
        //other scripts will be able to access the reference to this singleton if they need one.
        //If it is not in Awake, there may be a race condition (depending on script execution order)
        //to get the correct value of instance.
        _instance = this;
        //TODO initialize skill tree with prefab (based on class?)
        abilityManager = transform.parent.GetComponent<AbilityManager>();
        skillTree = GameObject.Find("Skill Tree Panel").transform;
        activeSkills = skillTree.FindChild("Active Skill Slots");
        skillTree = skillTree.FindChild("Skill Tree Slots");
        //CR: Again, we have access to the Player.instance singleton
        player = GetComponentInParent<Player>();
        //CR: We already have a Player.instance variable that has access to the player.
        //Use Player.instance.gameObject instead.
        playerGO = GameObject.Find("Player");

        for(int i = 0; i < maxAbilities; i++)
        {
            slots.Add("Empty");
            slotSprites.Add("Empty");
        }
    }

    //CR: As it turns out, omitting the Update() function from a MonoBehavior actually gives the game better
    //performance, even if the Update() function is empty. It's slight and I'm nitpicking here, but it's
    //also a fun fact.
    void Update()
    {
    }
    
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
    }

    //CR: This function would also become more readable with a MonoBehavior for slots.
    public void LoadSlots(List<string> newSlots, List<string> slotSprites)
    {
        slots = newSlots;
        for(int i = 0; i < activeSkills.childCount; i++)
        {
            Transform skill = activeSkills.GetChild(i).GetChild(0);
            skill.name = newSlots[i];
            Sprite sprite = Resources.Load("Items/" + slotSprites[i], typeof(Sprite)) as Sprite;
            skill.GetComponent<UnityEngine.UI.Image>().sprite = sprite;

            //CR: skill.gameObject.SetActive(newSlots[i] != "Empty");
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

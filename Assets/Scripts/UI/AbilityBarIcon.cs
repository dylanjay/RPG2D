using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityBarIcon : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Text keybindingText;

    [SerializeField]
    Image cooldownFillImage;

    private Animator animator;
    
    public CastableAbility ability { get; private set; }

    public string keybinding;
    public bool slotted = false;
    public int slot;

    GameObject playerGO;

    SkillTree skillTree;
    AbilityManager abilityManager;

    void Awake ()
    {
        animator = GetComponent<Animator>();
	}

    void Start()
    {
        skillTree = SkillTree.instance;
        abilityManager = AbilityManager.instance;
        playerGO = Player.instance.gameObject;
    }

    public void SetAbility(CastableAbility newAbility)
    {
        if (ability != null)
        {
            ability.onCast -= AbilityPressed;
        }
        ability = newAbility;
        ability.onCast += AbilityPressed;
        animator.SetFloat("inverseSpeed", 1.0f / (ability.cooldownLength + ability.castTime + ability.abilityDuration));
    }

    public void RemoveAbility(CastableAbility remAbility)
    {
        if (ability != remAbility)
        {
            Debug.LogError("Error: the ability being removed was not assigned to this AbilityBarIcon");
        }
        else
        {
            ability.onCast -= AbilityPressed;
        }
    }

    private void AbilityPressed()
    {
        animator.SetTrigger("cooldown");
    }

    public void AbilityKeybindingChanged(string newKeybinding)
    {
        keybindingText.text = newKeybinding;
    }

    void SwapAbilities(AbilityBarIcon other)
    {
        if(!slotted && !other.slotted)
        {
            return;
        }

        else if(slotted && !other.slotted)
        {
            CastableAbility tempAbility = ability;
            abilityManager.DisableAbility(ability, slot);
            abilityManager.EnableAbility(tempAbility, playerGO, other.slot);
        }

        else if(!slotted && other.slotted)
        {
            CastableAbility tempAbility = other.ability;
            abilityManager.DisableAbility(other.ability, other.slot);
            abilityManager.EnableAbility(tempAbility, playerGO, slot);
        }

        else if(slotted && other.slotted)
        {
            CastableAbility otherTempAbility = ability;
            CastableAbility thisTempAbility = other.ability;
            abilityManager.DisableAbility(ability, slot);
            abilityManager.DisableAbility(other.ability, other.slot);
            abilityManager.EnableAbility(thisTempAbility, playerGO, slot);
            abilityManager.EnableAbility(otherTempAbility, playerGO, other.slot);
        }
    }

    void AssignSlot()
    {
        SkillTreeNode node = skillTree.GetNode(skillTree.selectedSkillSlot);
        string skillName = node.skillName;
        Ability ability = node.ability;
        if (!abilityManager.isEquipped(node.skillName))
        {
            if (ability is CastableAbility)
            {
                CastableAbility castableAbility = (CastableAbility)ability;
                castableAbility.keybinding = keybinding;
            }
            abilityManager.EnableAbility(ability, playerGO, slot);
        }
        skillTree.skillSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (skillTree.skillSelected)
        {
            AssignSlot();
        }

        else if(!skillTree.swapSlot)
        {
            skillTree.swapSlot = true;
            skillTree.selectedSlotIndex = slot;
        }

        else if(skillTree.swapSlot)
        {
            skillTree.swapSlot = false;
            SwapAbilities(abilityManager.skillSlots[skillTree.selectedSlotIndex]);
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

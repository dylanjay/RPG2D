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

    // Use this for initialization
    void Awake () {
        animator = GetComponent<Animator>();
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

    public void OnPointerDown(PointerEventData eventData)
    {
        //if skill slot already selected
        if (skillTree.slotSelected)
        {
            //cancel skill slot selection if clicked itself again
            skillTree.slotSelected = false;

            //swap with other skill slot if selected another
            if (skillTree.selectedSlotIndex != slot)
            {
                SwapAbilities(abilityManager.skillSlots[skillTree.selectedSlotIndex]);
            }
        }

        //else select slot
        else
        {
            skillTree.selectedSlotIndex = slot;
            skillTree.slotSelected = true;
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

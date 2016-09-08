using UnityEngine;
using System.Collections;
using System;

public abstract class CastableAbility : Ability
{
    public string keybinding = "";

    [Tooltip("How long it takes to cast the ability.")]
    public float castTime = 0f;

    [Tooltip("How long the ability lasts. After this, the ability will go on cooldown.")]
    public float abilityDuration = 0f;

    [Tooltip("How long the ability stays on cooldown.")]
    public float cooldownLength = 1f;

    [Tooltip("The resource used to cast the ability.")]
    public string resourceUsed = "Mana";

    [Tooltip("The amount of the resource used. Can be set to zero.")]
    public float resourceCost = 10f;


    [HideInInspector]
    public float beginningCooldownTime = 0;
    private bool available = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        beginningCooldownTime = float.MinValue;
        available = true;
    }

    protected sealed override IEnumerator OnAbilityPressed()
    {
        base.OnAbilityPressed();

        if (available)
        {
            available = false;
            return AbilityCastSequence();
        }
        else
        {
            return null;
        }
    }

    private IEnumerator AbilityCastSequence()
    {
        //TODO: spend resource here.
        if(castTime > 0)
        {
            yield return new WaitForSeconds(castTime);
        }
        
        yield return OnAbilityCast();

        if (cooldownLength > 0)
        {
            Debug.Log("Begin Cooldown");
            yield return new WaitForSeconds(cooldownLength);
        }
        available = true;
        Debug.Log("Cooldown Finished");
        yield return null;
    }

    protected abstract IEnumerator OnAbilityCast();
}
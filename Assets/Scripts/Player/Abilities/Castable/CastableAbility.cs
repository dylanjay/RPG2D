using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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

    [Tooltip("The amount of time after using this ability that the character cannot use other abilities.")]
    public float abilityLockoutTime = 0f;

    [Tooltip("Whether or not movement should be locked for the duration of the ability.")]
    public bool lockMovement = true;

    public delegate void OnCast();
    [NonSerialized]
    public OnCast onCast;


    protected override void OnEnable()
    {
        base.OnEnable();
        _available = true;
    }

    public sealed override List<AbilityCallback> AbilityPressed()
    {
        base.AbilityPressed();

        if (_available)
        {
            _available = false;
            onCast();
            return abilityCallbacks;
        }
        else
        {
            return null;
        }
    }

    protected sealed override IEnumerator AbilityCastSequence()
    {
        Player.instance.GetComponent<AbilityManager>().LockoutAbilities(abilityLockoutTime);

        //TODO: spend resource here.
        if(castTime > 0)
        {
            yield return new WaitForSeconds(castTime);
        }

        if(lockMovement)
        {
            Player.instance.GetComponent<PlayerControl>().lockMovement = true;
        }
        yield return OnAbilityCast();
        if (lockMovement)
        {
            Player.instance.GetComponent<PlayerControl>().lockMovement = false;
        }

        if (cooldownLength > 0)
        {
            yield return new WaitForSeconds(cooldownLength);
        }
        _available = true;
    }

    protected abstract IEnumerator OnAbilityCast();
}
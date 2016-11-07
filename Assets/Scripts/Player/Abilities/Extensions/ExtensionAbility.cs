using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExtensionAbility<T> : Ability where T : Ability
{
    [SerializeField]
    protected T ability;

    protected override void OnAbilityDisable()
    {
        abilityCallbacks.Remove(AbilityCastSequence);
        abilityCallbacks = null;
    }

    protected override void OnAbilityEnable()
    {
        ability.abilityCallbacks.Add(AbilityCastSequence);
        abilityCallbacks = ability.abilityCallbacks;
        abilityCallbacks.Add(AbilityCastSequence);
    }
}

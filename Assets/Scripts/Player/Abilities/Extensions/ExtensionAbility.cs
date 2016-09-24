using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExtensionAbility<T> : Ability where T : Ability
{
    protected T ability;

    protected override void OnAbilityDisable()
    {
        abilityCallbacks.Remove(AbilityCastSequence);
        abilityCallbacks = null;
    }

    protected override void OnAbilityEnable()
    {
        ability.abilityCallbacks.Add(AbilityCastSequence);
        ability = (T)AbilityManager.allAbilities[typeof(T)];

        abilityCallbacks = ability.abilityCallbacks;
        abilityCallbacks.Add(AbilityCastSequence);
    }
}

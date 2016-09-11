using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExtensionAbility<T> : Ability where T : Ability
{
    protected T ability;

    protected override void OnAbilityDisable()
    {
        ability.abilityCallbacks.Remove(AbilityCastSequence);
    }

    protected override void OnAbilityEnable()
    {
        ability = (T)AbilityManager.allAbilities[typeof(T)];
        ability.abilityCallbacks.Add(AbilityCastSequence);
    }
}

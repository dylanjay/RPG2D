using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "New Fire Roll", menuName = "Abilities/Extensions/Fire Trail (Roll)", order = 0)]
public class FireRoll : ExtensionAbility<A_Roll>
{
    protected override IEnumerator AbilityCastSequence()
    {
        if (ability.castTime + ability.abilityDuration > 0)
        {
            yield return new WaitForSeconds(ability.castTime + ability.abilityDuration);
        }
    }
}

using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Sword Slash", menuName = "Abilities/Sword Slash", order = 0)]
public class A_SwordSlash : CastableAbility
{
    protected override void OnAbilityEnable()
    {

    }

    protected override void OnAbilityDisable()
    {

    }

    protected override IEnumerator OnAbilityCast()
    {
        //Beginning
        Player.instance.GetComponent<SpriteRenderer>().color /= 1.5f;

        PlayerControl.instance.anim.SetBool(PlayerControl.AnimParamIDs[(int)PlayerControl.AnimParams.Swing], true);
        
        yield return new WaitForSeconds(abilityDuration);

        //End
        Player.instance.GetComponent<SpriteRenderer>().color *= 1.5f;
    }
}
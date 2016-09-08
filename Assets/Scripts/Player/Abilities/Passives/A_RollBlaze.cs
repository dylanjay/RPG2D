using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Roll Blaze", menuName = "Abilities/Roll Blaze", order = 0)]
public class A_RollBlaze : Ability
{
    public float burnWidth = 1f;

    public float burnDamagePerSecond = 1f;

    protected override void OnAbilityEnable()
    {
        A_Roll roll = Player.instance.GetComponent<AbilityManager>().GetAbility(typeof(A_Roll)) as A_Roll;

        roll.leaveAfterBurn = true;
    }
    protected override void OnAbilityDisable()
    {
        A_Roll roll = Player.instance.GetComponent<AbilityManager>().GetAbility(typeof(A_Roll)) as A_Roll;

        roll.leaveAfterBurn = false;
    }

}

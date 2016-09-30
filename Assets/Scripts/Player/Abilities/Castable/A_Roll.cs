using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "New Roll", menuName = "Abilities/Roll", order = 0)]
public class A_Roll : CastableAbility
{
    protected override void OnAbilityEnable()
    {

    }

    protected override void OnAbilityDisable()
    {

    }

    /*protected override IEnumerator OnAbilityCast()
    {
        //Beginning
        Player.instance.GetComponent<SpriteRenderer>().color /= 1.5f;
        Vector2 movementVec = Player.instance.GetComponent<PlayerControl>().lastDirection * 10;
        float beginTime = Time.time;
        WaitForFixedUpdate waitForEndOfFrame = new WaitForFixedUpdate();
        
        //Ability loop
        while (Time.time - beginTime < abilityDuration)
        {
            Player.instance.transform.Translate(movementVec * Time.fixedDeltaTime);
            yield return waitForEndOfFrame;
        }

        //End
        Player.instance.GetComponent<SpriteRenderer>().color *= 1.5f;
    }*/

    //Physics based roll. Huge initial speed and then falls off due to friction.
    protected override IEnumerator OnAbilityCast()
    {
        Player.instance.GetComponent<SpriteRenderer>().color /= 1.5f;
        Player.instance.GetComponent<Rigidbody2D>().velocity = Player.instance.GetComponent<PlayerControl>().lastDirection * 100;
        yield return new WaitForSeconds(abilityDuration);
        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Player.instance.GetComponent<SpriteRenderer>().color *= 1.5f;
    }

    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }
}
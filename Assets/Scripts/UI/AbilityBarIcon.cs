using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityBarIcon : MonoBehaviour {

    [SerializeField]
    Text keybindingText;

    [SerializeField]
    Image cooldownFillImage;

    private Animator animator;
    
    public CastableAbility ability { get; private set; }

    // Use this for initialization
    void Awake () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetAbility(CastableAbility newAbility)
    {
        if (ability != null)
        {
            ability.onCast -= AbilityPressed;
        }
        ability = newAbility;
        ability.onCast += AbilityPressed;
        animator.SetFloat("inverseSpeed", 1.0f / (ability.cooldownLength + ability.castTime));
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
}

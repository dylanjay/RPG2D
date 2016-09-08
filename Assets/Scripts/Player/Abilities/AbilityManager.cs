using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AbilityManager : MonoBehaviour {

    /// <summary>
    /// Used for testing purposes only! Will be removed during actual use.
    /// 
    /// Acts as a list of abilities that are being asked to be equipped.
    /// </summary>
    public List<Ability> abilities;


    private List<Ability> equippedAbilities  = new List<Ability>();

    private List<string> keybindingKeys = new List<string>();
    private Dictionary<string, CastableAbility> keybindingsToAbilities = new Dictionary<string, CastableAbility>();

    void Awake()
    {
        equippedAbilities = new List<Ability>(abilities);
        foreach(Ability ability in abilities)
        {
            EnableAbility(ability);
        }
        //Probably want to load in the abilities here.
    }

    void Update()
    {
        foreach (string keybinding in keybindingKeys)
        {
            CastableAbility castedAbility;
            if(keybindingsToAbilities.TryGetValue(keybinding, out castedAbility))
            {
                if(Input.GetButtonDown(castedAbility.keybinding))
                {
                    Debug.Log("Pressed");
                    IEnumerator iEnumerator = castedAbility.Activate();
                    if(iEnumerator != null)
                    {
                        StartCoroutine(iEnumerator);
                    }
                }
            }
        }
    }

    public void DisableAbility(Ability ability)
    {
        if(ability is CastableAbility)
        {
            RemoveKeybinding((CastableAbility)ability);
        }
        equippedAbilities.Remove(ability);
        ability.Disable();
    }

    public void RemoveKeybinding(CastableAbility ability)
    {
        if (ability.keybinding != "")
        {
            keybindingsToAbilities.Remove(ability.keybinding);
            keybindingKeys.Remove(ability.keybinding);
            ability.keybinding = "";
        }
    }

    public void AddKeybinding(CastableAbility ability, string keybinding)
    {
        if(ability.keybinding != "")
        {
            RemoveKeybinding(ability);
        }
        ability.keybinding = keybinding;
        keybindingsToAbilities.Add(keybinding, ability);
        keybindingKeys.Add(keybinding);
    }

    public void EnableAbility(Ability ability)
    {
        //Set the ability to the same keybind as last time if set previously.
        //If something is already in that slot, remove the keybinding.
        if(ability is CastableAbility)
        {
            CastableAbility castableAbility = (CastableAbility)ability;
            if (keybindingsToAbilities.ContainsKey(castableAbility.keybinding))
            {
                castableAbility.keybinding = "";
            }
            else if (castableAbility.keybinding != "")
            {
                AddKeybinding(castableAbility, castableAbility.keybinding);
            }
        }
        

        //Add the ability to the list and enable it.
        equippedAbilities.Add(ability);
        ability.Enable();
        
    }

    public Ability GetAbility(string ability)
    {
        return equippedAbilities.Find(x => x.name == ability);
    }

    public Ability GetAbility(Type abilityType)
    {
        return equippedAbilities.Find(x => x.GetType() == abilityType);
    }

    void OnDestroy()
    {
        for(int i = 0; i < equippedAbilities.Count; i++)
        {
            equippedAbilities[i].Disable();
        }
    }
}

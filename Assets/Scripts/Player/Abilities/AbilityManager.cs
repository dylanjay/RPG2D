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
    /// 

    [SerializeField]
    private List<Ability> abilities;

    [SerializeField]
    private Transform abilityDisplayBar;

    [SerializeField]
    private GameObject abilityDisplayPrefab;

    public static Dictionary<Type, Ability> allAbilities = new Dictionary<Type, Ability>();
    public Dictionary<string, Ability> abilityDict = new Dictionary<string, Ability>();

    [HideInInspector]
    public List<Ability> equippedAbilities  = new List<Ability>();

    private List<string> keybindingKeys = new List<string>();
    private Dictionary<string, CastableAbility> keybindingsToAbilities = new Dictionary<string, CastableAbility>();

    private float lockoutTimer = 0;

    public int maxEquippedAbilities;

    PlayerControl player;

    void Awake()
    {
        //equippedAbilities = new List<Ability>(abilities);
        foreach(Ability ability in abilities)
        {
            //EnableAbility(ability, gameObject);
            //ability.gameObject = gameObject;
            abilityDict.Add(ability.name, ability);
        }
        //Probably want to load in the abilities here.
    }

    void Start()
    {
        player = PlayerControl.instance;
    }

    void Update()
    {
        lockoutTimer -= Time.deltaTime;
        if (lockoutTimer > 0) { return; }

        if (!player.lockMovement)
        {
            foreach (string keybinding in keybindingKeys)
            {
                CastableAbility castedAbility;
                if (keybindingsToAbilities.TryGetValue(keybinding, out castedAbility))
                {
                    if (Input.GetButtonDown(castedAbility.keybinding))
                    {
                        List<Ability.AbilityCallback> abilityCallbacks = castedAbility.AbilityPressed();
                        if (abilityCallbacks != null)
                        {
                            foreach (Ability.AbilityCallback abilityCallback in abilityCallbacks)
                            {
                                if (abilityCallback != null)
                                {
                                    StartCoroutine(abilityCallback());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void DisableAbility(Ability ability)
    {
        if (equippedAbilities.Count > 0)
        {
            if (ability is CastableAbility)
            {
                RemoveKeybinding((CastableAbility)ability);
            }
            equippedAbilities.Remove(ability);
            ability.Disable();
        }
    }

    public void RemoveKeybinding(CastableAbility ability)
    {
        if (ability.keybinding != "")
        {
            keybindingsToAbilities.Remove(ability.keybinding);
            keybindingKeys.Remove(ability.keybinding);
            ability.keybinding = "";
        }

        //Remove from to Ability Bar
        foreach (AbilityBarIcon child in abilityDisplayBar.GetComponentsInChildren<AbilityBarIcon>())
        {
            if(child.ability == ability)
            {
                //TODO: Remove ability's callbacks to destroyed object.
                //TODO: Possibly remove destroy by pooling objects.
                Destroy(child.gameObject);
            }
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

        //Add to Ability Bar
        GameObject abilityDisplay = Instantiate(abilityDisplayPrefab);
        abilityDisplay.GetComponent<AbilityBarIcon>().SetAbility(ability);
        abilityDisplay.transform.SetParent(abilityDisplayBar);
    }

    public void EnableAbility(Ability ability, GameObject target)
    {
        //Set the ability to the same keybind as last time if set previously.
        //If something is already in that slot, remove the keybinding.
        ability.gameObject = target;
        if (equippedAbilities.Count < maxEquippedAbilities)
        {
            if (ability is CastableAbility)
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
    }

    public Ability GetAbility(string ability)
    {
        return equippedAbilities.Find(x => x.name == ability);
    }

    public Ability GetAbility(Type abilityType)
    {
        return equippedAbilities.Find(x => x.GetType() == abilityType);
    }

    public bool isEquipped(string name)
    {
        return equippedAbilities.Contains(GetAbility(name));
    }

    public void LockoutAbilities(float lockoutTime)
    {
        //Make the lockout timer the greater of the two (current lockoutTimer or lockoutTime).
        if (lockoutTimer < lockoutTime)
        {
            lockoutTimer = lockoutTime;
        }
    }

    /*void OnDestroy()
    {
        for(int i = 0; i < equippedAbilities.Count; i++)
        {
            equippedAbilities[i].Disable();
        }
    }*/
}

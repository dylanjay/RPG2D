using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ability : ScriptableObject{

    [System.NonSerialized]
    public GameObject gameObject;

    [HideInInspector]
    private bool enabled = false;

    [TextArea(3, 10)]
    public string description = "";

    public delegate IEnumerator AbilityCallback();

    [HideInInspector]
    public List<AbilityCallback> abilityCallbacks;

    protected bool _available = true;

    public bool available
    {
        get { return _available; }
    }

    protected virtual void OnEnable()
    {
        enabled = false;

        abilityCallbacks = new List<AbilityCallback>();
        abilityCallbacks.Add(AbilityCastSequence);
    }

    public void Enable()
    {
        Debug.Assert(!enabled, "Error: Ability " + name + " is already equipped.");
        enabled = true;
        OnAbilityEnable();
    }

    public void Disable()
    {
        Debug.Assert(enabled, "Error: Ability " + name + " is not equipped.");
        enabled = false;
        OnAbilityDisable();
    }

    protected abstract void OnAbilityEnable();

    protected abstract void OnAbilityDisable();

    public virtual List<AbilityCallback> AbilityPressed() { return null; }

    protected abstract IEnumerator AbilityCastSequence();
}

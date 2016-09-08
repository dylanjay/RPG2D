using UnityEngine;
using System.Collections;

public abstract class Ability : ScriptableObject{

    [HideInInspector]
    private bool enabled = false;

    [TextArea(3, 10)]
    public string description = "";

    protected virtual void OnEnable()
    {
        enabled = false;
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
        OnAbilityEnable();
    }

    public IEnumerator Activate()
    {
        return OnAbilityPressed();
    }
    
    protected abstract void OnAbilityEnable();

    protected abstract void OnAbilityDisable();

    protected virtual IEnumerator OnAbilityPressed() { return null; }
}

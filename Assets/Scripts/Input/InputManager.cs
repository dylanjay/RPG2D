using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using KeyBinding = System.String;

public delegate void KeyBindingCallback();
public delegate void GetKeyPressed(KeyCode keyCode);

/// <remarks>
/// Note: This class can be an actual DontDestroyOnLoad Singleton, due to the nature of it not needing
/// any of it's MonoBehavior API other than Update() and OnGUI()
/// </remarks>
public class InputManager : Singleton<InputManager>
{
    /// <summary>
    /// This variable is a set of all keys currently pressed down.
    /// </summary>
    private static HashSet<KeyCode> keysDown = new HashSet<KeyCode>();

    /// <summary>
    /// This dictionary maps the keycodes of the actual keys with strings for key names. This allows for 
    /// multiple keys to be mapped to the same action, but does not allow multiple actions to be mapped
    /// to the same key. Any keys that should never be mapped to should be added to this dictionary in the
    /// constructor to prevent any mapping to those keys.
    /// </summary>
    private static Dictionary<KeyCode, KeyBinding> keyBindings = new Dictionary<KeyCode, KeyBinding>();
    
    private static Dictionary<KeyBinding, KeyBindingCallback> onKey = new Dictionary<string, KeyBindingCallback>();
    private static Dictionary<KeyBinding, KeyBindingCallback> onKeyUp = new Dictionary<string, KeyBindingCallback>();
    private static Dictionary<KeyBinding, KeyBindingCallback> onKeyDown = new Dictionary<string, KeyBindingCallback>();

    private static GetKeyPressed keyListenerDelegate = null;
	
    void Awake()
    {
        //Any keys that should be prevented from being mapped to by the user should be added here as such:
        keyBindings.Add(KeyCode.Escape, "Escape");

        keyBindings.Add(KeyCode.W, "Up");
        keyBindings.Add(KeyCode.S, "Down");
        keyBindings.Add(KeyCode.A, "Left");
        keyBindings.Add(KeyCode.D, "Right");

        keyBindings.Add(KeyCode.Space, "Ability 1");
        keyBindings.Add(KeyCode.LeftShift, "Ability 2");
        keyBindings.Add(KeyCode.Q, "Ability 3");
        keyBindings.Add(KeyCode.F, "Ability 4");
    }

	// Update is called once per frame
	void OnGUI()
    {
        Event current = Event.current;
        if(current.isKey)
        {
            if (current.type == EventType.KeyDown && current.keyCode != KeyCode.None)
            {
                SetKeyDown(current.keyCode);
            }
            else if(current.type == EventType.KeyUp)
            {
                SetKeyUp(current.keyCode);
            }
        }
    }
    
    //Currently, there is no way to use the Event system to determine which shift is being pressed.
    //To check for these keys, we must use Input.GetKey and handle them in Update().
    void Update()
    {
        SpecialCharacterUpdate(KeyCode.LeftShift);
        SpecialCharacterUpdate(KeyCode.RightShift);
        InvokeOnKeyDelegates();
    }

    private void InvokeOnKeyDelegates()
    {
        foreach (KeyCode keyCode in keysDown)
        {
            if (keyBindings.ContainsKey(keyCode))
            {
                string keyBinding = keyBindings[keyCode];
                if (onKey.ContainsKey(keyBinding))
                {
                    onKey[keyBinding]();
                }
            }
        }
    }

    //A function to handle any characters  Event.current does not handle.
    private void SpecialCharacterUpdate(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode))
        {
            SetKeyDown(keyCode);
        }
        else if(Input.GetKeyUp(keyCode))
        {
            SetKeyUp(keyCode);
        }
    }

    private void SetKeyDown(KeyCode keyCode)
    {
        //If a keypress is being listened to, return a keypress that isn't mapped.
        if (keyListenerDelegate != null && !keysDown.Contains(keyCode))
        {
            //Using this tempInvoker variable allows the invoked callback to call GetNextKeyPress() inside it.
            GetKeyPressed tempInvoker = keyListenerDelegate;
            keyListenerDelegate = null;
            tempInvoker(keyCode);
        }

        if (keyBindings.ContainsKey(keyCode) && keysDown.Add(keyCode))
        {
            string buttonName = keyBindings[keyCode];

            if(onKeyDown.ContainsKey(buttonName))
            {
                onKeyDown[buttonName]();
            }
        }
    }

    private void SetKeyUp(KeyCode keyCode)
    {
        if (keyBindings.ContainsKey(keyCode) && keysDown.Remove(keyCode))
        {
            KeyBinding buttonName = keyBindings[keyCode];

            if (onKeyUp.ContainsKey(buttonName))
            {
                onKeyUp[buttonName]();
            }
        }
    }

    public static void AddBindingOnKey(KeyBinding keyBinding, KeyBindingCallback callback)
    {
        if(onKey.ContainsKey(keyBinding))
        {
            onKey[keyBinding] += callback;
        }
        else
        {
            onKey.Add(keyBinding, callback);
        }
    }

    public static void AddBindingOnKeyUp(KeyBinding keyBinding, KeyBindingCallback callback)
    {
        if (onKeyUp.ContainsKey(keyBinding))
        {
            onKeyUp[keyBinding] += callback;
        }
        else
        {
            onKeyUp.Add(keyBinding, callback);
        }
    }

    public static void AddBindingOnKeyDown(KeyBinding keyBinding, KeyBindingCallback callback)
    {
        if (onKeyDown.ContainsKey(keyBinding))
        {
            onKeyDown[keyBinding] += callback;
        }
        else
        {
            onKeyDown.Add(keyBinding, callback);
        }
    }

    public static void RemoveBindingOnKey(KeyBinding keyBinding, KeyBindingCallback callback)
    {
        if (!onKey.ContainsKey(keyBinding))
        {
            Debug.LogWarning("Warning: No keybinding found. RemoveKeyBindingOnKey call will be ignored.");
            return;
        }
        onKey[keyBinding] -= callback;
        if(onKey[keyBinding].GetInvocationList().Length == 0)
        {
            onKey.Remove(keyBinding);
        }
    }

    public static void RemoveBindingOnKeyUp(KeyBinding keyBinding, KeyBindingCallback callback)
    {
        if (!onKeyUp.ContainsKey(keyBinding))
        {
            Debug.LogWarning("Warning: No keybinding found. RemoveKeyBindingOnKeyUp call will be ignored.");
            return;
        }
        onKeyUp[keyBinding] -= callback;
        if (onKeyUp[keyBinding].GetInvocationList().Length == 0)
        {
            onKeyUp.Remove(keyBinding);
        }
    }

    public static void RemoveBindingOnKeyDown(KeyBinding keyBinding, KeyBindingCallback callback)
    {
        if (!onKeyDown.ContainsKey(keyBinding))
        {
            Debug.LogWarning("Warning: No keybinding found. RemoveKeyBindingOnKeyUp call will be ignored.");
            return;
        }
        onKeyDown[keyBinding] -= callback;
        if (onKeyDown[keyBinding].GetInvocationList().Length == 0)
        {
            onKeyDown.Remove(keyBinding);
        }
    }

    /// <summary>
    /// Adds a listener for an unused KeyCode. After the next key is pressed, the listener is cleared.
    /// Note: Only one function is allowed to listen at a time. The callback function passed in
    /// will override any previously set callback.
    /// </summary>
    /// <param name="callback">The function that recieves the new KeyCode.</param>
    public static void GetNextKeyPress(GetKeyPressed callback)
    {
        keyListenerDelegate = callback;
    }

    //public static bool IsKeyDown(KeyBinding keybinding) { }
}

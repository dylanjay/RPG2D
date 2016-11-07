using UnityEngine;
using System.Collections;
using KeyBinding = System.String;

public class InputAxes
{
    private static InputAxis _playerVertical = new InputAxis("Up", "Down");
    public static InputAxis playerVertical { get { return _playerVertical; } }

    private static InputAxis _playerHorizontal = new InputAxis("Right", "Left");
    public static InputAxis playerHorizontal { get { return _playerHorizontal; } }
}

public class InputAxis
{
    bool backPressed = false;
    bool forwardPressed = false;

    public int value
    {
        get { return (backPressed ? -1 : 0) + (forwardPressed ? 1 : 0); }
    }

    KeyBinding forward, backward;

    private void OnForwardKeyUp() { forwardPressed = false; }
    private void OnForwardKeyDown() { forwardPressed = true; }
    private void OnBackKeyUp() { backPressed = false; }
    private void OnBackKeyDown() { backPressed = true; }

    public InputAxis(KeyBinding forward, KeyBinding backward)
    {
        this.forward = forward;
        this.backward = backward;
        InputManager.AddBindingOnKeyUp(forward, OnForwardKeyUp);
        InputManager.AddBindingOnKeyDown(forward, OnForwardKeyDown);
        InputManager.AddBindingOnKeyUp(backward, OnBackKeyUp);
        InputManager.AddBindingOnKeyDown(backward, OnBackKeyDown);
    }

    ~InputAxis()
    {
        InputManager.AddBindingOnKeyUp(forward, OnForwardKeyUp);
        InputManager.AddBindingOnKeyDown(forward, OnForwardKeyDown);
        InputManager.AddBindingOnKeyUp(backward, OnBackKeyUp);
        InputManager.AddBindingOnKeyDown(backward, OnBackKeyDown);
    }

    public static implicit operator int (InputAxis inputAxis)
    {
        return inputAxis.value;
    }
}

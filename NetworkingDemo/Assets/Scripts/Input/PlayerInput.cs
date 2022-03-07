
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    PlayerInputActions pia;

    sbyte inputVertical;
    sbyte inputHorizontal;
    public InputSerialization.DirectionalInput input;


    private void Awake()
    {    
       pia = new PlayerInputActions();
       pia.Player.Enable();
       pia.Player.SetCallbacks(this);    
    }

    public void OnVertical(InputAction.CallbackContext context)
    {
        OnDirection(context, true);
    }

    public void OnHorizontal(InputAction.CallbackContext context)
    {
        OnDirection(context, false);
    }

    private void OnDirection(InputAction.CallbackContext context, bool isVertical)
    {
        if (context.performed) return;
        sbyte value = (sbyte)context.ReadValue<float>();
        if (isVertical) inputVertical = value; else inputHorizontal = value;
        input = InputSerialization.ConvertInputAxisToDirectionalInput(inputHorizontal, inputVertical);
        Debug.Log(input);
    }

    public void OnPunch(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnKick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnHSlash(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}

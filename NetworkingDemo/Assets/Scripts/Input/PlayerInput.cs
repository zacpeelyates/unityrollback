
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    [SerializeField]
    NetworkManager networkManager;

    PlayerInputActions pia;

    sbyte inputVertical;
    sbyte inputHorizontal;
    public InputSerialization.DirectionalInput dirInput;
    InputSerialization.Inputs InputThisFrame;
    ushort frame = 0; 
    
    private InputSerialization.ButtonInputType ContextToInputType(InputAction.CallbackContext context)
    {
        if (context.started) return InputSerialization.ButtonInputType.BINPUT_PRESSED;
        if (context.performed) return InputSerialization.ButtonInputType.BINPUT_HELD;
        if (context.canceled) return InputSerialization.ButtonInputType.BINPUT_RELEASED;
        return InputSerialization.ButtonInputType.BINPUT_NONE;
    }

    private void Awake()
    {    
       pia = new PlayerInputActions();
       pia.Player.Enable();
       pia.Player.SetCallbacks(this);    
    }

    private void Update()
    {
        InputSerialization.DirectionalInput d = InputSerialization.DirectionalInput.DINPUT_NEUTRAL;
        if (InputThisFrame != null) d = InputThisFrame.dir;
        InputThisFrame = new InputSerialization.Inputs(frame) { dir = d  };
        frame++;
    }

    private void LateUpdate()
    {
        if (!GameSimulation.isAlive) return;
        GameSimulation.AddLocalInput(InputThisFrame); //give local input to game sim
        networkManager.SendMessage(InputSerialization.Inputs.ToBytes(InputThisFrame)); //send input to remote    
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
        dirInput = InputSerialization.ConvertInputAxisToDirectionalInput(inputHorizontal, inputVertical);
       // Debug.Log(dirInput);
        InputThisFrame.dir = dirInput;
       
    }

    public void OnPunch(InputAction.CallbackContext context)
    {
        InputThisFrame.buttons[(int)InputSerialization.ButtonID.BUTTON_PUNCH] = ContextToInputType(context);
    }

    public void OnKick(InputAction.CallbackContext context)
    {
        InputThisFrame.buttons[(int)InputSerialization.ButtonID.BUTTON_KICK] = ContextToInputType(context);
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        InputThisFrame.buttons[(int)InputSerialization.ButtonID.BUTTON_SLASH] = ContextToInputType(context);
    }

    public void OnHSlash(InputAction.CallbackContext context)
    {
        InputThisFrame.buttons[(int)InputSerialization.ButtonID.BUTTON_HSLASH] = ContextToInputType(context);
    }

    public void OnTestMessage(InputAction.CallbackContext context)
    {
        networkManager.SendMessage(new byte[] { 1, 2, 3, 4 });
    }
}

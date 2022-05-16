
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
    bool[] isHeld = new bool[(int)InputSerialization.ButtonID.BUTTON_COUNT];
    [SerializeField]public const int INPUT_DELAY = 2;
    
   
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
        InputThisFrame = new InputSerialization.Inputs((ushort)(GameSimulation.LocalFrame+INPUT_DELAY)) { dir = d  };
    }

    private void LateUpdate()
    {
        if (!GameSimulation.isAlive) return;
        for(int i = 0; i < isHeld.Length; ++i )
        {
            if (isHeld[i])
            {
                InputThisFrame.buttons[i] = InputSerialization.ButtonInputType.BINPUT_HELD; //new unity system doesnt work good for holding so this is a hacky workaround
            }
        }
        GameSimulation.AddLocalInput(InputThisFrame); //give local input to game sim
        networkManager.SendMessage(InputSerialization.Inputs.ToBytes(InputThisFrame)); //send input to remote    
    }




    private void OnDirection(InputAction.CallbackContext context, bool isVertical)
    {
        if (context.performed) return;
        sbyte value = (sbyte)context.ReadValue<float>();
        if (isVertical) inputVertical = value;
        else inputHorizontal = value;
        dirInput = InputSerialization.ConvertInputAxisToDirectionalInput(inputHorizontal, inputVertical);
        // Debug.Log(dirInput);
        InputThisFrame.dir = dirInput;
    }


    public void OnVertical(InputAction.CallbackContext context) => OnDirection(context, true);

    public void OnHorizontal(InputAction.CallbackContext context) => OnDirection(context, false);

    private InputSerialization.ButtonInputType ContextToInputType(InputAction.CallbackContext context, int index)
    {
        if (context.canceled)
        {
            isHeld[index] = false;
            return InputSerialization.ButtonInputType.BINPUT_RELEASED;
        }

        if (context.started)
        {
            isHeld[index] = true;
            return InputSerialization.ButtonInputType.BINPUT_PRESSED;
        }    

        if (isHeld[index])
        {
            return InputSerialization.ButtonInputType.BINPUT_HELD;
        }

        return InputSerialization.ButtonInputType.BINPUT_NONE;
    }

    public void OnButton(InputAction.CallbackContext context, int index) => InputThisFrame.buttons[index] = ContextToInputType(context, index);
    
    public void OnPunch(InputAction.CallbackContext context) => OnButton(context, (int)InputSerialization.ButtonID.BUTTON_PUNCH);

    public void OnKick(InputAction.CallbackContext context) => OnButton(context, (int)InputSerialization.ButtonID.BUTTON_KICK);

    public void OnSlash(InputAction.CallbackContext context) => OnButton(context, (int)InputSerialization.ButtonID.BUTTON_SLASH);

    public void OnHSlash(InputAction.CallbackContext context) => OnButton(context, (int)InputSerialization.ButtonID.BUTTON_HSLASH);
 

    public void OnTestMessage(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Debug.Log("testing rollback");
        GameSimulation.LoadPreviousGamestate(512);
    }
}

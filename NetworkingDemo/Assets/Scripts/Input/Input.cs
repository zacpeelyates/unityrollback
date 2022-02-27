using System.Collections;
using System.Collections.Generic;
public static class Input
{
    public enum DirectionalInput //Basic numpad directional notation
    { 
        DINPUT_UNKNOWN, //used when we don't recieve any input data from remote player for this frame
        DINPUT_DOWN_BACK, 
        DINPUT_DOWN,
        DINPUT_DOWN_FWD,
        DINPUT_BACK,
        DINPUT_NEUTRAL,
        DINPUT_FWD,
        DINPUT_UP_BACK,
        DINPUT_UP,
        DINPUT_UP_FWD

        /*
         * Numpad Notation:
         * 
         * 7 8 9 
         * 4 5 6 
         * 1 2 3 
         * 
         * 5 is neutral/no input, numbers about neutral denote direction (e.g 2 = DOWN, 9 = UP_RIGHT, 6 = FWD)
         */
    }


    public static DirectionalInput FlipHorizonal(DirectionalInput d) //flips back inputs to fwd inputs and vice-versa - neutral input not affected
    {
        int i = (int)d;
        if (i == 1 || i == 4 || i == 7) i += 2;
        else if (i == 3 || i == 6 || i == 9) i -= 2;
        return (DirectionalInput)i;
    }

    public enum ButtonInputType
    {
        BINPUT_NONE, //no input for this button this frame
        BINPUT_PRESSED, //button was pressed this frame
        BINPUT_HELD, //button was pressed in a previous frame, and has not yet been released
        BINPUT_RELEASED, //button was released this frame (useful for negative edge)
        BINPUT_COUNT
    }

    public enum ButtonID
    {
        BUTTON_PUNCH,
        BUTTON_KICK,
        BUTTON_SLASH,
        BUTTON_HSLASH,
        BUTTON_COUNT
    }

    public struct Button
    {
        public ButtonID id;
        public ButtonInputType input;
    }

    public static byte[] InputToByte(DirectionalInput dir, List<Button> buttons)
    {
        /*
         *  each button is 4 bytes (2 for ID, 2 for type)
         *  motion input is 4 bytes (need to store 0-9 for 8 dir + neutral)
         *  each byte in returned byte[] will contain a pair of button inputs
         *  the final byte will contain the motion input data with 4 unused bits
         *  if there is an odd number of buttons, we can pack these unused bits with the final button data
         */

        List<byte> byteList = new List<byte>();
        for (int i = 0; i < buttons.Count; i += 2)
        {
            byte buttonPair = 0;
            for (int j = 0; j < 2; ++j)
            {
                //create a byte containing a pair of button information
                byte buttonInfo = ButtonTo4Bit(buttons[i + j]);
                buttonPair |= (byte)(buttonInfo << j * 4);
            }
            byteList.Add(buttonPair);
        }
        byte last = 0;
        if (buttons.Count % 2 != 0) last |= (byte)(ButtonTo4Bit(buttons[buttons.Count - 1]) << 4); //add non-paired button input
        last |= (byte)dir; //add motion input data
        byteList.Add(last);

        return byteList.ToArray();
        
    }
           
    public static byte ButtonTo4Bit(Button b) => (byte)(((byte)b.id << 2) | (byte)b.input);
    

}

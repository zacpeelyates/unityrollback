using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class InputSerialization
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



    public static DirectionalInput ConvertInputAxisToDirectionalInput(sbyte horizontal, sbyte vertical)
    {
        //if the values are not -1, 0, or 1 something has gone terribly wrong
        if (horizontal > 1 || horizontal < -1 || vertical > 1 || vertical < -1) return DirectionalInput.DINPUT_UNKNOWN;
        return DirectionalInput.DINPUT_NEUTRAL + horizontal + (3 * vertical); //see numpad notation above
        //e.g. if horizontal is -1 (back) and vertical is 1 (up) result = 5 - 1 + 3 = 7 (up+back)
    }

    public static (sbyte horizontal, sbyte vertical) ConvertDirectionalInputToAxis(DirectionalInput dir)
    {
        //reverse of above method, converts numpad notation to positive + vertical axis
        int d = (int)dir;
        int h = (d % 3) - 2; //horizontal
        if (System.Math.Abs(h) > 1) h = -System.Math.Sign(h); //"overflow" (2 should be -1, -2 should be 1)
        int v = (d - 5 - h) / 3; //vertical
        return ((sbyte)h, (sbyte)v); //return as tuple

        /*
         *  example:
         *  d = 3 (down + forward)
         *  horizontal = (3 % 3) -2 = -2
         *  horizontal gets corrected to 1
         *  vertical = (3 - 5 - 1) / 3 = -3/3 = -1
         *  
         *  returns horizontal 1, vertical -1 
         *  we can check this against the reverse method, 5 + h + 3v = 5 + 1 - 3 = 3 which is the d we started with
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


    public static byte[] InputToBytes(Inputs inputs)
    {
        /*
         *  each button is 4 bytes (2 for ID, 2 for type)
         *  motion input is 4 bytes (need to store 0-9 for 8 dir + neutral)
         *  each byte in returned byte[] will contain a pair of button inputs
         *  the final byte will contain the motion input data with 4 unused bits
         *  if there is an odd number of buttons, we can pack these unused bits with the final button data
         */

        List<byte> byteList = new List<byte>();
        for (int i = 0; i < inputs.buttons.Count; i += 2)
        {
            byte buttonPair = 0;
            for (int j = 0; j < 2; ++j)
            {
                //create a byte containing a pair of button information
                byte buttonInfo = ButtonTo4Bit(inputs.buttons[i + j]);
                buttonPair |= (byte)(buttonInfo << j * 4);
            }
            byteList.Add(buttonPair);
        }
        byte last = 0;
        if (inputs.buttons.Count % 2 != 0) last |= (byte)(ButtonTo4Bit(inputs.buttons.Last()) << 4); //add non-paired button input
        last |= (byte)inputs.dir; //add motion input data
        byteList.Add(last);

        return byteList.ToArray();

    }

    public static Inputs BytesToInputs(List<byte> b)
    {
        Inputs i;
        i.buttons = null; //todo
        i.dir = (DirectionalInput) ((b.Last() << 4) >> 4); //use bitshifts to remove unwanted bits

        return i;

    }


    public struct Inputs
    {
        public DirectionalInput dir;
        public List<Button> buttons;
    }


    public static byte ButtonTo4Bit(Button b) => (byte)(((byte)b.id << 2) | (byte)b.input);
    

}

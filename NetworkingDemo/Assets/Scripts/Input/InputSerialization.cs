using System;
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
        int v = (d - (int)DirectionalInput.DINPUT_NEUTRAL - h) / 3; //vertical
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
        if (i % 3 == 1) i += 2;
        else if (i % 3 == 0) i -= 2;
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

    public class Inputs
    {
        public ushort FrameID;
        public DirectionalInput dir;
        public ButtonInputType[] buttons = new ButtonInputType[(int)ButtonInputType.BINPUT_COUNT];

        enum INPUT_OFFSETS
        { 
            ID,
            BUTTONS = 2,
            DIRECTIONAL
        }

        public Inputs() { }
        public Inputs(ushort ID)
        {
            FrameID = ID;
            dir = DirectionalInput.DINPUT_UNKNOWN;
            buttons = new ButtonInputType[(int)ButtonID.BUTTON_COUNT];
        }

        public const int MessageSizeInBytes = 4;

        public static byte[] ToBytes(Inputs inputs)
        {
            /*
             *  Array begins with frameID (2 bytes), then:                                  
             *  each button is 2 bits (buttoninputtype) in order of ButtonIDs               
             *  motion input is 4 bits (need to store 0-9 for 8 dir + neutral)              
             *  each byte in returned byte[] will contain 4 button inputs
             *  the final byte will contain the motion input data with 4 unused bits
             *  If we have 5 or 6 buttons in future, we can pack them in these unused bits
             */
            List<byte> result = new List<byte>
            {
                (byte)((inputs.FrameID >> 8) & 0xFF), //first 8 bits of 16bit frameID
                (byte)(inputs.FrameID & 0xFF) //second 8 bits of 16 bit frame id
            };

            byte next = 0;

            for(int i = (int)ButtonID.BUTTON_COUNT-1; i >= 0; --i )
            {
                next |= (byte)((byte)inputs.buttons[i] << (i << 1));
            }

            result.Add(next);
            result.Add((byte)inputs.dir);
            return result.ToArray();
        }

        public static Inputs FromBytes(byte[] bytes)
        {
            Inputs result = new Inputs
            {
                FrameID = (ushort)((bytes[(ushort)INPUT_OFFSETS.ID] << 8) | bytes[(ushort)INPUT_OFFSETS.ID+1]), //first two bytes make up 16 bit representation of frameID      
                dir = (DirectionalInput)(bytes[(ushort)INPUT_OFFSETS.DIRECTIONAL] & 0xF) //dir is final 4 bits 
            };

            for(int i = 0; i < (int)ButtonID.BUTTON_COUNT; ++i)
            {
                result.buttons[i] = (ButtonInputType)(bytes[(int)INPUT_OFFSETS.BUTTONS] & (0xC0 >> (i << 1))); //mask 2-bit values until end of byte
            }

            return result;
        }
    }
    public class FrameInfo
    {
        private Inputs local, remote;
        public bool remoteIsPredicted = false;
        public Inputs GetLocalInputs() => local;
        public Inputs GetRemoteInputs() => remote;
        public void SetLocalInputs(Inputs i) => local = i;
        public void SetRemoteInputs(Inputs i) => remote = i;


        public FrameInfo ReturnWithNewInput(Inputs i, bool isRemote)
        {
            if (isRemote) remote = i;          
            else local = i;
            return this;
        }
        
    }
}

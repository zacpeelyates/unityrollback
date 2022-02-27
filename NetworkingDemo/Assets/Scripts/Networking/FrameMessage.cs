using System.Collections;
using System.Collections.Generic;

public class FrameInfo
{
    private readonly ushort ID;
    private readonly byte[] inputs;

    public FrameInfo(ushort ID, byte[] inputs)
    {
        this.ID = ID;
        this.inputs = inputs;
    }
    public ushort GetFrameID() { return ID;}
    public byte[] GetInputs() { return inputs; }


    public static FrameInfo FromBytes(List<byte> bytes)
    {
        ushort ID = (ushort)(bytes[0] << 8 + bytes[1]);
        bytes.RemoveRange(0, 2); //remove ID from bytes;
        byte[] inputs = bytes.ToArray();
        return new FrameInfo(ID, inputs);     
    }

    public static List<byte> ToBytes(FrameInfo f)
    {
        List<byte> bytes = new List<byte>
        {
            (byte)f.ID,
            (byte)(f.ID >> 8)
            //ID
        };
        bytes.AddRange(f.inputs); //Inputs
        return bytes;
        
    }

}



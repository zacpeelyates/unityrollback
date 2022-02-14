using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public static class NetworkUtils
{
    public static byte[] StreamToBytes(Stream stream, int initialLength = byte.MaxValue) //adapted from jonskeet.uk/csharp/readbinary.html
    {
        if (initialLength < 1) initialLength = byte.MaxValue;
        //setup buffer
        byte[] buffer = new byte[initialLength];
        int count = 0;
        int chunk;
        while((chunk = stream.Read(buffer,count,buffer.Length-count)) > 0) //read from stream
        {
            count += chunk;
            if(count == buffer.Length) //we have filled the buffer
            {
                int next = stream.ReadByte();
                if (next == -1) return buffer; //stream is finished, return here
                //stream not finished, increase buffer size
                byte[] newBuffer = new byte[buffer.Length * 2]; 
                Array.Copy(buffer, newBuffer, buffer.Length);
                newBuffer[count] = (byte)next;
                buffer = newBuffer;
                count++;
            }
        }
        //shrink buffer to size of stream and return
        byte[] result = new byte[count];
        Array.Copy(buffer, result, count);
        return result;
    }
}
    


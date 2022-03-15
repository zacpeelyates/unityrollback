using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System;

public class GameSimulation
{
     static GameState current;
     public static bool isAlive = false;
     public static ushort currentFrame;
     public static ConcurrentDictionary<ushort, InputSerialization.FrameInfo> FrameDictionary;
     public static uint framesToProcess;
     const ushort MAX_FRAME_BUFFER = 7;

    private static void Init(bool p1Local)
    {
        current = new GameState(p1Local);
        currentFrame = 0;
        FrameDictionary = new ConcurrentDictionary<ushort, InputSerialization.FrameInfo>();
        framesToProcess = 0;
        isAlive = true;
    }

    public static void AddRemoteInput(InputSerialization.Inputs remoteInput)
    {
        InputSerialization.FrameInfo temp = new InputSerialization.FrameInfo();
        temp.SetRemoteInputs(remoteInput);
        FrameDictionary.AddOrUpdate(remoteInput.FrameID, temp, (k,v) => v.ReturnWithNewRemote(remoteInput));
        Console.WriteLine("Added network input to frame: " + remoteInput.FrameID);
    }

    public static void AddLocalInput(InputSerialization.Inputs localInput)
    {
        InputSerialization.FrameInfo temp = new InputSerialization.FrameInfo();
        temp.SetLocalInputs(localInput);
        FrameDictionary.TryAdd(localInput.FrameID, temp);
    }

    private static void SimulateForward(GameState g, uint frames)
    {
        if (frames == 0) return;
        
        SimulateForward(g, frames - 1);
    }

    public static void Run(bool p1Local)
    {
        Init(p1Local);
        while(isAlive) //update loop
        {
            if (framesToProcess > 0)
            {            
                FrameDictionary.TryGetValue(currentFrame, out InputSerialization.FrameInfo f);
                if (f == null) Console.WriteLine("Couldn't get current frame info!");
                current.Tick(f);
                framesToProcess--;
                currentFrame++;

                Transport.current = current;
            }
            if(currentFrame > MAX_FRAME_BUFFER)
            {
                FrameDictionary.TryRemove((ushort)(currentFrame - MAX_FRAME_BUFFER), out _);
            }
        }
    }
   


}

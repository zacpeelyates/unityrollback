using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;

public class GameSimulation
{
     static GameState current;
     static bool isAlive;
     public static uint currentFrame;
     public static ConcurrentDictionary<int, (FrameInfo local, FrameInfo remote)> FrameDictionary;
    const InputSerialization.DirectionalInput baseInput = InputSerialization.DirectionalInput.DINPUT_UNKNOWN; 

    private static void Init()
    {
        current = new GameState();
        isAlive = true;
        currentFrame = 0;
        FrameDictionary = new ConcurrentDictionary<int, (FrameInfo local, FrameInfo remote)>();
    }

    private static void SimulateForward(GameState g, uint frames)
    {
        if (frames == 0) return;
        
        SimulateForward(g, frames - 1);
    }

    public static void Run()
    {
        Init();
        double dt = 1000.0 / 60.0;
        double t;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        while(isAlive) //update loop
        {
            t = sw.ElapsedMilliseconds;
            if (t >= dt)
            {
                
                currentFrame++;
                InputSerialization.DirectionalInput[] inputs = new InputSerialization.DirectionalInput[2] { baseInput, baseInput };
                if (FrameDictionary.TryGetValue((int)currentFrame,out (FrameInfo local, FrameInfo remote) d))
                {
                    inputs[0] = d.local.GetDirectionalInput();
                    inputs[1] = d.remote.GetDirectionalInput();
                }

                current.Tick(inputs);
                sw.Restart();
            }
        }
    }
   


}

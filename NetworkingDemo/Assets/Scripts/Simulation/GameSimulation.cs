using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;

public class GameSimulation
{
     static GameState current;
     static bool isAlive;
     public static ushort currentFrame;
     public static ConcurrentDictionary<ushort, FrameInfo> FrameDictionary;
    const InputSerialization.DirectionalInput baseInput = InputSerialization.DirectionalInput.DINPUT_UNKNOWN; 

    private static void Init()
    {
        current = new GameState();
        isAlive = true;
        currentFrame = 0;
        FrameDictionary = new ConcurrentDictionary<ushort, FrameInfo>();
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
                if (FrameDictionary.TryGetValue(currentFrame, out FrameInfo f))
                {
                    inputs[0] = f.GetLocalInputs().dir;
                    inputs[1] = f.GetRemoteInputs().dir;
                }

                current.Tick(inputs);
                sw.Restart();
            }
        }
    }
   


}

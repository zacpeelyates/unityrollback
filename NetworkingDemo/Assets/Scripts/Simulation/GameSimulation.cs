using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

public class GameSimulation
{
     static GameState current;
     public static bool isAlive = false;
     public static ushort localFrame;
     public static ConcurrentDictionary<ushort, InputSerialization.FrameInfo> FrameDictionary;
     public static uint framesToProcess;
     const ushort MAX_FRAME_BUFFER = 7;
     private static HashSet<ushort> RollbackFrames;
    private static Dictionary<int, GameState> GameStateDictionary;
     
     

    private static InputSerialization.Inputs LastRemoteInputRecieved;
    private static ushort lastRemoteFrame => (ushort)(LastRemoteInputRecieved == null ? 0 : LastRemoteInputRecieved.FrameID);
    private static void Init(bool p1Local)
    {
        current = new GameState(p1Local);
        localFrame = 0;
        FrameDictionary = new ConcurrentDictionary<ushort, InputSerialization.FrameInfo>();
        framesToProcess = 0;
        isAlive = true;
        RollbackFrames = new HashSet<ushort>();
        GameStateDictionary = new Dictionary<int, GameState>();
    }

    public static void AddLocalInput(InputSerialization.Inputs input)
    {
        InputSerialization.FrameInfo temp = new InputSerialization.FrameInfo();
        temp.SetLocalInputs(input);
        FrameDictionary.AddOrUpdate(input.FrameID, temp, (k, v) => v.ReturnWithNewInput(input, false));
    }

    public static void AddRemoteInput(InputSerialization.Inputs input, bool isPredicted)
    {
        if (!isPredicted) LastRemoteInputRecieved = input;
        InputSerialization.FrameInfo temp = new InputSerialization.FrameInfo();
        temp.SetRemoteInputs(input);
        temp.remoteIsPredicted = isPredicted;
        FrameDictionary.AddOrUpdate(input.FrameID, temp, (k, v) =>
        {
            if(!isPredicted && v.remoteIsPredicted)
            {
                InputSerialization.Inputs existingInput;
                if((existingInput = v.GetRemoteInputs()) != input) //predicted input was wrong
                {
                    //we are overwriting an input prediction -- must rollback to correct our mispredict
                    RollbackFrames.Add(input.FrameID);
                }
            }
            else v.remoteIsPredicted = isPredicted;
            return v.ReturnWithNewInput(input, true);
        });
    }

    public static void Run(bool p1Local)
    {
        Init(p1Local);
        current.frameID = localFrame;
        while(isAlive) //update loop
        {
            if (framesToProcess > 0)
            {
                framesToProcess--;
                PredictRemoteInputs(localFrame - lastRemoteFrame);
                if(RollbackFrames.Count != 0) current = HandleRollbacks();
                FrameDictionary.TryGetValue(localFrame, out InputSerialization.FrameInfo f);
                current = current.Tick(f);
                localFrame++;
                GameStateDictionary.Add(current.frameID, current);
                ushort earliestBufferedFrame = (ushort)(current.frameID - MAX_FRAME_BUFFER);
                GameStateDictionary.Remove(earliestBufferedFrame);
                FrameDictionary.TryRemove(earliestBufferedFrame, out _);
                Transport.current = current;
            }
        }
    }

    private static GameState HandleRollbacks()
    {
        GameState g = null;
        lock (GameStateDictionary)
        {
            GameStateDictionary.TryGetValue(RollbackFrames.Min(), out g);
        }
        if (g == null) return current;
        for (int i = g.frameID; i <= localFrame;)
        {
            FrameDictionary.TryGetValue((ushort)i, out InputSerialization.FrameInfo f);
            g = g.Tick(f);
        }
        return g;        
    }

    private static void PredictRemoteInputs(int localFrameAdvantage)
    {
       if (localFrameAdvantage <= 0 || LastRemoteInputRecieved == null) return;
       InputSerialization.Inputs predictedRemote = LastRemoteInputRecieved;
       for (int i = 1; i <= localFrameAdvantage; ++i) //fill in missing remote inputs
       {
         predictedRemote.FrameID = (ushort)(lastRemoteFrame + i);
         AddRemoteInput(predictedRemote, true);
       }
            
    }

}

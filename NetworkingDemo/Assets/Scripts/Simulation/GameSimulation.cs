using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System;

public class GameSimulation
{
    static GameState current;
    public static bool isAlive = false;
    public static ConcurrentDictionary<ushort, InputSerialization.FrameInfo> FrameInputDictionary;
    const ushort MAX_FRAME_BUFFER = 8;
    private static HashSet<ushort> RollbackFrames;
    private static Dictionary<int, GameState> GameStateDictionary;
    public static uint rollbackCount = 0;



    private static InputSerialization.Inputs LastRemoteInputRecieved;

    public static int LocalFrame => current == null ? -1 : current.frameID;
    public static int LastRemoteFrame => LastRemoteInputRecieved == null ? -1 : LastRemoteInputRecieved.FrameID;
    private static void Init(bool p1Local)
    {
        current = new GameState(p1Local);
        FrameInputDictionary = new ConcurrentDictionary<ushort, InputSerialization.FrameInfo>();
        isAlive = true;
        RollbackFrames = new HashSet<ushort>();
        GameStateDictionary = new Dictionary<int, GameState>();
    }

    public static void AddLocalInput(InputSerialization.Inputs input)
    {
        InputSerialization.FrameInfo temp = new InputSerialization.FrameInfo();
        temp.SetLocalInputs(input);
        FrameInputDictionary.AddOrUpdate(input.FrameID, temp, (k, v) => v.ReturnWithNewInput(input, false));
    }

    public static void AddRemoteInput(InputSerialization.Inputs input, bool isPredicted)
    {
        if (!isPredicted) LastRemoteInputRecieved = input;
        InputSerialization.FrameInfo temp = new InputSerialization.FrameInfo();
        temp.SetRemoteInputs(input);
        temp.remoteIsPredicted = isPredicted;
        FrameInputDictionary.AddOrUpdate(input.FrameID, temp, (k, v) =>
        {
            if (!isPredicted && v.remoteIsPredicted)
            {
                InputSerialization.Inputs existingInput;
                if ((existingInput = v.GetRemoteInputs()) != input) //predicted input was wrong
                {
                    //we are overwriting an input prediction -- must rollback to correct our mispredict
                    RollbackFrames.Add(input.FrameID);
                }
            }
            else v.remoteIsPredicted = isPredicted;
            return v.ReturnWithNewInput(input, true);
        });
    }


    readonly static long TICKS_PER_FRAME = 166667; //16.67ms for 60fps
    public static void Run(bool p1Local)
    {
        Init(p1Local);
        current.frameID = 0;
        long prev = System.DateTime.UtcNow.Ticks;
        long lag = 0;


        while (isAlive) //update loop
        {
          long now = System.DateTime.UtcNow.Ticks;
            long elapsed = now - prev;
            prev = now;
            lag += elapsed;
            while (lag >= TICKS_PER_FRAME) //lets us update many times if we lag behind
            {
                lag -= TICKS_PER_FRAME;
                //handle rollbacks
                if (RollbackFrames.Count > 0) current = HandleRollbacks();
                //get inputs for this frame
                FrameInputDictionary.TryGetValue((ushort)current.frameID, out InputSerialization.FrameInfo frameInputs);
                //predict remote inputs
                PredictRemoteInputs(current.frameID - LastRemoteFrame);
                //update gamestate
                current = current.Tick(frameInputs);
                //store gamestate in buffer
                GameStateDictionary.Add(current.frameID, current); //must copy ctor in otherwise value updates with current for some godforsaken reason
                //send gamestate to unity main thread / renderer
                Transport.current = current;
                //cleanup
                ushort earliestBufferedFrame = (ushort)(current.frameID - MAX_FRAME_BUFFER);
                FrameInputDictionary.TryRemove(earliestBufferedFrame, out _);
                GameStateDictionary.Remove(earliestBufferedFrame);
            
            }
        }
    }

    private static GameState HandleRollbacks()
    {
        GameState g = null;
        lock (GameStateDictionary)
        {
            GameStateDictionary.TryGetValue(RollbackFrames.Min(), out g);
            RollbackFrames.Clear();
        }
        if (g == null) return current;
        for (int i = g.frameID; i < current.frameID;)
        {
            FrameInputDictionary.TryGetValue((ushort)i, out InputSerialization.FrameInfo f);
            g = g.Tick(f);
        }
        rollbackCount++;
        return g;
    }

    private static void PredictRemoteInputs(int localFrameAdvantage)
    {
        if (localFrameAdvantage <= 0 || LastRemoteInputRecieved == null) return;
        InputSerialization.Inputs predictedRemote = LastRemoteInputRecieved;
        for (int i = 1; i <= localFrameAdvantage; ++i) //fill in missing remote inputs
        {
            //update until we are back at current frame
            predictedRemote.FrameID = (ushort)(LastRemoteFrame + i);
            AddRemoteInput(predictedRemote, true);
        }
    }

    public static void LoadPreviousGamestate(ushort FrameID)
    {
        lock (GameStateDictionary)
        {
            lock (current)
            {
                current = GameStateDictionary[FrameID];
            }
        }

    }
}

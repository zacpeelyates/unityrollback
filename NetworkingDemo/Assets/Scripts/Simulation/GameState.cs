using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
   public SimPlayer[] players = new SimPlayer[2];

    static readonly FInt32 STARTPOS = 2;

    public GameState(bool p1Local)
    {
        players[0] = new SimPlayer(!p1Local); //local player
        players[1] = new SimPlayer(p1Local); //remote player

        players[0].pos.x = -STARTPOS;
        players[1].pos.x = STARTPOS;
    }

    public void Tick(InputSerialization.FrameInfo f)
    {
       if (f == null) return;
       foreach(SimPlayer s in players)
       {
           if(s!=null) s.ApplyInput(s.isRemote ? f.GetRemoteInputs() : f.GetLocalInputs());
       }
    }
}

public class SimPlayer
{
    public FVec2 pos;
    public bool isRemote;

    public SimPlayer(bool remote)
    {
        pos.x = 0;
        pos.y = 0;
        isRemote = remote;
    }

    public void ApplyInput(InputSerialization.Inputs i)
    {
       if (i == null) return;
       (sbyte h, sbyte v) = InputSerialization.ConvertDirectionalInputToAxis(i.dir);
       pos.x += h * moveSpeed;
       pos.y += v * moveSpeed;    
    }

    public static readonly FInt32 moveSpeed = FInt32.FromString("0.1");
}


public struct FVec2
{
    public FInt32 x, y;

    public FVec2(FInt32 xpos, FInt32 ypos) 
    {
        x = xpos;
        y = ypos;
    }

    public Vector3 ToVec3(float zPos) => new Vector3(x.ToFloat, y.ToFloat, zPos);
    public FInt32 Magnitude => FInt32.Sqrt(FInt32.Pow(x, 2) + FInt32.Pow(y, 2));

    public static FInt32 Distance(FVec2 a, FVec2 b) => FInt32.Sqrt(FInt32.Pow(a.x - b.x, 2) + FInt32.Pow(a.y - b.y, 2));
}


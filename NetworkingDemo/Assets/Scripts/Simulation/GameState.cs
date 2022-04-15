using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public SimPlayer[] players = new SimPlayer[2];

    static readonly FInt32 STARTPOS = 2;
    static readonly FInt32 GRAVITY = FInt32.FromString("0.1");

    public int frameID;

    public GameState(bool p1Local)
    {
        players[0] = new SimPlayer(!p1Local); //local player
        players[1] = new SimPlayer(p1Local); //remote player

        players[0].pos.x = -STARTPOS;
        players[1].pos.x = STARTPOS;

        frameID = 0;
    }

    public GameState Tick(InputSerialization.FrameInfo f)
    {
       GameState next = this;
       next.frameID = frameID + 1;
       foreach (SimPlayer s in next.players)
        {
            if (s == null) continue;

            if (s.IsGrounded)
            {
                s.vel.y = 0;
                s.pos.y = 0;
            } else s.vel.y -= GRAVITY;

            if (f != null)s.ApplyInput(s.isRemote ? f.GetRemoteInputs() : f.GetLocalInputs());

            
            s.pos += s.vel;
        }
       return next;
    }
}

public class SimPlayer
{
    public static readonly FInt32 GROUND = 0;
    public FVec2 pos;
    public FVec2 vel;
    public bool isRemote;
    public bool IsGrounded => pos.y <= GROUND;


    public SimPlayer(bool remote)
    {
        pos = new FVec2(0, 0);
        vel = new FVec2(0, 0);
        isRemote = remote;
    }



    public void ApplyInput(InputSerialization.Inputs i)
    {
       if (i == null) return;
       (sbyte h, sbyte v) = InputSerialization.ConvertDirectionalInputToAxis(i.dir);
        FInt32 temp = vel.x;
        vel.x += h * moveSpeed;
        if (v > 0 && IsGrounded)
        {
            vel.y += v * 2;
        }
        if (v < 0 || h == 0 || FInt32.Abs(temp) > FInt32.Abs(vel.x))
        {
            vel.x = 0;
            return;
        }


       vel.x = FInt32.Clamp(vel.x, -maxMovespeed,maxMovespeed);
    }

    public static readonly FInt32 moveSpeed = FInt32.FromString("0.01");
    public static readonly FInt32 maxMovespeed = FInt32.FromString("0.1");
    
}





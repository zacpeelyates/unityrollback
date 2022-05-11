using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public SimPlayer[] players = new SimPlayer[2];

    static readonly FInt32 STARTPOS = 2 + FInt32.HALF;
    public InputSerialization.FrameInfo cachedInfo;

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
       cachedInfo = f;
       GameState next = this;
       next.frameID = frameID + 1;
       foreach (SimPlayer s in next.players)
        {
            if (s == null) continue;

            if (s.IsGrounded)
            {
                s.pos.y = 0;
                s.vel.y = 0;
                s.state = PlayerState.PS_IDLE;
            } else
            {
                s.state = PlayerState.PS_AIRBORNE;
            }

            if (f != null)s.ApplyInput(s.isRemote ? f.GetRemoteInputs() : f.GetLocalInputs());
            
            s.pos += s.vel;
        }

        bool direction = players[0].pos.x < players[1].pos.x;
        players[0].facingLeft = !direction; 
        players[1].facingLeft = direction; 
        

       return next;
    }
}

public class SimPlayer
{
    public static readonly FInt32 GROUND = 0;
    public static readonly FInt32 JUMP = FInt32.FromString("0.125");
    public FVec2 pos;
    public FVec2 vel;
    public bool isRemote;
    public bool IsGrounded => pos.y <= GROUND;
    public PlayerState state;
    public static readonly FInt32 GRAVITY_PREAPEX = FInt32.FromString("0.0075");
    public static readonly FInt32 GRAVITY_POSTAPEX = FInt32.FromString("0.075");
    public static readonly FInt32 APEX = 2;
    public bool facingLeft;
    public FInt32 FORWARD => facingLeft ? -1 : 1;



    public SimPlayer(bool remote)
    {
        pos = new FVec2(0, 0);
        vel = new FVec2(0, 0);
        isRemote = remote;
        state = PlayerState.PS_IDLE;
    }



    public void ApplyInput(InputSerialization.Inputs i)
    {
        if (i == null) return;
        (sbyte h, sbyte v) = InputSerialization.ConvertDirectionalInputToAxis(i.dir);
        FInt32 temp = vel.x;
        vel.y += IsGrounded ? v>0 ? v*JUMP : 0 : pos.y >= APEX || vel.y < 0 ? -GRAVITY_POSTAPEX : -GRAVITY_PREAPEX;
        vel.x += IsGrounded ? h * moveSpeed : 0;
        vel.x = FInt32.Clamp(vel.x, -maxMovespeed, maxMovespeed);

        if (IsGrounded && h != 0) state = PlayerState.PS_WALK;
        if (IsGrounded && v < 0) state = PlayerState.PS_CROUCH;
        if (state == PlayerState.PS_CROUCH || IsGrounded && h == 0 || FInt32.Abs(temp) > FInt32.Abs(vel.x)) vel.x = 0;

        if (IsGrounded && i.buttons[(int)InputSerialization.ButtonID.BUTTON_PUNCH] == InputSerialization.ButtonInputType.BINPUT_HELD) state = PlayerState.PS_PUNCH;
        if (IsGrounded && i.buttons[(int)InputSerialization.ButtonID.BUTTON_KICK] == InputSerialization.ButtonInputType.BINPUT_HELD) state = PlayerState.PS_KICK;
        if (IsGrounded && i.buttons[(int)InputSerialization.ButtonID.BUTTON_SLASH] == InputSerialization.ButtonInputType.BINPUT_HELD) state = PlayerState.PS_SLASH;
        if (IsGrounded && i.buttons[(int)InputSerialization.ButtonID.BUTTON_HSLASH] == InputSerialization.ButtonInputType.BINPUT_HELD) state = PlayerState.PS_HSLASH;
        if (state == PlayerState.PS_CROUCH  || IsGrounded && h == 0|| FInt32.Abs(temp) > FInt32.Abs(vel.x)) vel.x = 0;

    }

    public static readonly FInt32 moveSpeed = FInt32.FromString("0.01");
    public static readonly FInt32 maxMovespeed = FInt32.FromString("0.02");

}

public enum PlayerState
{ 
    PS_IDLE,
    PS_WALK,
    PS_CROUCH,
    PS_PUNCH,
    PS_KICK,
    PS_SLASH,
    PS_HSLASH,
    PS_AIRBORNE,


    PS_COUNT
}







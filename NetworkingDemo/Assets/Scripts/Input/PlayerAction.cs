using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction
{
    public abstract void Execute(Player owner);
}

public class ActionWalkBack : PlayerAction
{
    public override void Execute(Player owner)
    {
        //implement
    }
}

public class ActionWalkForward : PlayerAction
{
    public override void Execute(Player owner)
    {
        //implement
    }
}

public class ActionCrouch : PlayerAction
{
    public override void Execute(Player owner)
    {
        //implement
    }
}

public class ActionJump : PlayerAction
{
    public override void Execute(Player owner)
    {
        //implement
    }
}

public class ActionPunch : PlayerAction
{
    public override void Execute(Player owner)
    {
       
    }
}

public class ActionKick : PlayerAction
{
    public override void Execute(Player owner)
    {

    }
}
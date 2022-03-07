using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSimulation
{
     static GameState current;
    public static void Init()
    {
        current = new GameState();
    }

    void Tick()
    {
        //recieve new inputs


        //check and handle rollbacks

        //actual update
        current.Tick();
    }

    public static void SimulateForward(GameState g, uint frames)
    {
        if (frames == 0) return;
        
        SimulateForward(g, frames - 1);
    }
    


}

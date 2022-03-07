using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameTransport : MonoBehaviour
{
    Thread simulationThread;
    int frame = 0;

    private void Start()
    {
        simulationThread = new Thread(GameSimulation.Init);
    }

    private void FixedUpdate()
    {
        frame++;
    }

    static void SendInfoToSimulation(FrameInfo p1, FrameInfo p2)
    {
        if (p1.GetID() != p2.GetID())
        {
            Debug.LogError("frame IDs don't match!");
            return;
        }
    }

}

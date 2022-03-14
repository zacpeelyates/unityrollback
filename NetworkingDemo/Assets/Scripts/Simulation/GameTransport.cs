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
        simulationThread = new Thread(GameSimulation.Run);
        simulationThread.Start();
    }

    private void FixedUpdate()
    {
        frame++;
    }

}

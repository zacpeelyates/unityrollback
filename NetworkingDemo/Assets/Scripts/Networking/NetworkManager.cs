using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Peer))]
public class NetworkManager : MonoBehaviour
{
    public Peer localPeer;
    Thread GameThread;
    
    private void Start()
    {

        if (!localPeer) localPeer = GetComponent<Peer>();

        //setup delegates
        localPeer.outgoingConnectionSucceeded = OnOutgoingConnectionSucceeded;
        localPeer.outgoingConnectionFailed = OnOutgoingConnectionFailed;
        localPeer.allowRemoteConnections = OnAllowRemoteConnections;
        localPeer.recievedConnection = OnRecieveConnection;
        localPeer.peerDisconnected = OnPeerDisconnect;

        localPeer.InitClient();

    }

    private void FixedUpdate()
    {
        if (localPeer != null)
        {
            if (localPeer.messagesToSend.Count != 0) localPeer.Send();
            while (localPeer.recievedMessageQueue.Count > 0)
            {
                HandleMessage(localPeer.recievedMessageQueue.Dequeue());
            }
        }
    }

    public void SendMessage(byte[] message)
    {
        localPeer.messagesToSend.Enqueue(message);
    }

    void HandleMessage(byte[] message)
    {
        //send message to our game sim       
        GameSimulation.AddRemoteInput(InputSerialization.Inputs.FromBytes(message));    
    }

    void OnOutgoingConnectionSucceeded()
    {
        Debug.Log("ESTABLISHED CONNECTION");
        //Start game sim as client
        GameThread = new Thread(RunAsClient) { IsBackground = true };
    }

    private static void RunAsClient() => GameSimulation.Run(false); 
    private static void RunAsServer() => GameSimulation.Run(true);

    //with this setup host is always left side / player 1

    void OnOutgoingConnectionFailed()
    {
        Debug.Log("FAILED TO CONNECT");
        //Print error
    }

    void OnAllowRemoteConnections()
    {
        Debug.Log("SETTING UP SERVER");
    }

    void OnRecieveConnection()
    {
        Debug.Log("FOUND CLIENT");
        //Start game sim as server
        GameThread = new Thread(RunAsServer) { IsBackground = true };
    }

    void OnPeerDisconnect()
    {
        Debug.Log("LOST PEER");
        //Stop game sim
        GameSimulation.isAlive = false;
        GameThread.Abort();
    }

}

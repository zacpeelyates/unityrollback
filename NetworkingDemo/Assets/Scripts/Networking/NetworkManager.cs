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
                if(localPeer.recievedMessageQueue.TryDequeue(out byte[] message))
                {
                    HandleMessage(message);
                }              
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
        GameSimulation.AddRemoteInput(InputSerialization.Inputs.FromBytes(message),false);    
    }

    void OnOutgoingConnectionSucceeded()
    {
        Debug.Log("ESTABLISHED CONNECTION");
        CreateGameThread(true); //Start game sim as client
    }

    private static void RunAsClient() => GameSimulation.Run(false); 
    private static void RunAsServer() => GameSimulation.Run(true);

    //with this setup host is always left side / player 1

    void OnOutgoingConnectionFailed()
    {
        Debug.Log("FAILED TO CONNECT");
    }

    void OnAllowRemoteConnections()
    {
        Debug.Log("SETTING UP SERVER");
    }

    void OnRecieveConnection()
    {
        Debug.Log("FOUND CLIENT");
        CreateGameThread(false); //Start game sim as server
    }

    void CreateGameThread(bool isClient)
    {
        GameThread = isClient ? new Thread(RunAsClient) : new Thread(RunAsServer); //cant write the ternary operator in ctor because this is c# 8.0 and thats a >=9.0 feature
        GameThread.IsBackground = true;
        GameThread.Start();
    }

    void OnPeerDisconnect()
    {
        Debug.Log("LOST PEER");
        //Stop game sim
        GameSimulation.isAlive = false;
        GameThread.Abort();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Peer))]
public class NetworkManager : MonoBehaviour
{
    public Peer localPeer;
    
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

    private void Update()
    {
        if (localPeer != null)
        {
            if (localPeer.messagesToSend.Count != 0) localPeer.Send();
            while (localPeer.recievedMessageQueue.Count != 0)
            {
                HandleMessage(localPeer.recievedMessageQueue.Dequeue());
            }
        }
    }

    void HandleMessage(byte[] message)
    {
        //send message to our game sim?
    }

    void OnOutgoingConnectionSucceeded()
    {
        Debug.Log("ESTABLISHED CONNECTION");
        //Start game sim as client
    }

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
    }

    void OnPeerDisconnect()
    {
        Debug.Log("LOST PEER");
        //Stop game sim
    }

}

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
            while (localPeer.MessageBuffer.Count != 0)
            {
                Debug.Log("PARSING: " + localPeer.MessageBuffer.Dequeue());
            }
            localPeer.Send(new byte[] { 1, 0, 1, 0, 1, 0, 1, 1 });
        }
        else Debug.Log("Peer not found");
    }

    void OnOutgoingConnectionSucceeded()
    {
        Debug.Log("ESTABLISHED CONNECTION");
    }

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
    }

    void OnPeerDisconnect()
    {
        Debug.Log("LOST CLIENT");
    }

}

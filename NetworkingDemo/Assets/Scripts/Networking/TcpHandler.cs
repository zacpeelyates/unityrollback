using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(NetworkConfig))]
public class TcpHandler : MonoBehaviour
{
    #region Members
    public NetworkConfig config;
    //public Action delegates, assigned to outside of this class to allow for code to be called when an event triggers
    public Action outgoingConnectionSucceeded; //managed to connect to peer
    public Action outgoingConnectionFailed; //failed to connect to peer
    public Action allowRemoteConnections; //allow peer to connect to us
    public Action recievedConnection; //recieved a peer connetion
    public Action peerDisconnected; //peer disconnected

    protected TcpListener listener; //listen for connections
    protected TcpClient localClient; //establishes connection to peer

    private Queue<byte[]> MessageBuffer = new Queue<byte[]>();
    #endregion

    private void Awake()
    {
        config = GetComponent<NetworkConfig>();
    }

    public void Close()
    {
        listener.Stop();
        localClient.Close();
    }

    private void OnApplicationQuit()
    {
        Close();
    }

    #region Client
    public void InitClient()
    {
        localClient = new TcpClient(config.remoteIP, config.remotePort); //try to connect to remote peer
        if (localClient == null)
        {
            Debug.LogError("Failed to initialize client");
            return;
        }
        if (localClient.Connected)
        {
            Debug.Log("Connected");
            localClient.ReceiveTimeout = 1;
            outgoingConnectionSucceeded?.Invoke();
            CreateClientThread(localClient); //create new thread to handle connection
            
        }
        else
        {
            Debug.LogWarning("Failed to connect");
            outgoingConnectionFailed?.Invoke();
        }

    }

    private void CreateClientThread(TcpClient tcpClient)
    {
        Thread thread = new Thread(MessageRecieverTask){ IsBackground = true};
        thread.Start(tcpClient);
    }

    private void MessageRecieverTask(object obj)
    {
        TcpClient client = (TcpClient)obj;
        Debug.Log("Reciever thread active...");
        while(client.Connected)
        {
            byte[] message = NetworkUtils.StreamToBytes(client.GetStream());
            MessageBuffer.Enqueue(message);
            Debug.Log("recieved: " + message.ToString());
        }
    }


    public virtual void Send(byte[] message)
    {
        if (!localClient.Connected) peerDisconnected?.Invoke();
        else
        {
            localClient.GetStream().Write(message, 0, message.Length);
            Debug.Log("Sent " + message.ToString());
        }
    }

    #endregion

    #region Listener
    public void InitListener()
    {
        CreateListenerThread();
    }

    private void CreateListenerThread()
    {
        Thread listenerThread = new Thread(MessageListenerTask)  { IsBackground = true };
        listenerThread.Start();
        allowRemoteConnections?.Invoke();
    }

    private void MessageListenerTask()
    {
        listener = new TcpListener(IPAddress.Parse(config.localIP), config.listenPort);
        listener.Start();
        Debug.Log("listening...");
        TcpClient remoteClient = listener.AcceptTcpClient();
        recievedConnection?.Invoke();
        CreateClientThread(remoteClient);
    }
    #endregion
}
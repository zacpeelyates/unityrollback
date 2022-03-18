using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(NetworkConfig))]
public class Peer : MonoBehaviour
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

    public ConcurrentQueue<byte[]> recievedMessageQueue = new ConcurrentQueue<byte[]>();
    public ConcurrentQueue<byte[]> messagesToSend = new ConcurrentQueue<byte[]>();
    #endregion

    private void Awake()
    {
        if(!config) config = GetComponent<NetworkConfig>();
    }

    public void Close()
    {

        if(listener != null) listener.Stop();
        if(localClient != null) localClient.Close();
    }

    private void OnApplicationQuit()
    {
        Close();
    }

    #region Client
    public void InitClient()
    {
        if(localClient != null)
        {
            Debug.LogError($"Client already initialised at: {config.remoteIP}::{config.remotePort}");
            return;
        }

        try
        {
            localClient = new TcpClient(config.remoteIP, config.remotePort); //try to connect to remote peer 
        } 
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        if (localClient != null && localClient.Connected)
        {
            localClient.ReceiveTimeout = 1;
            outgoingConnectionSucceeded?.Invoke();
            CreateClientThread(localClient); //create new thread to handle connection
            
        }
        else
        {
            outgoingConnectionFailed?.Invoke();
            Debug.Log("Attempting to act as server as no server found...");
            config.SwapPorts();
            InitListener();
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
        client.NoDelay = true;
        //remove this, testing 
        client.SendTimeout = 10000;
        client.ReceiveTimeout = 10000;
        const int SIZE = InputSerialization.Inputs.MessageSizeInBytes;

        Debug.Log("Reciever thread active...");
        NetworkStream Stream = client.GetStream();
        byte[] message = new byte[SIZE];
        while (client.Connected)
        {
            if (Stream.CanRead)
            {
                
                if (Stream.ReadAsync(message, 0, SIZE).Result >= SIZE)
                {
                    recievedMessageQueue.Enqueue(message);
                    message = new byte[SIZE];
                    Stream.FlushAsync();
                }
            }
        }
        Stream.Close();
    }
    public virtual void Send()
    {
        if (localClient != null)
        {
            if (!localClient.Connected) peerDisconnected?.Invoke();
            else
            {            
                NetworkStream Stream = localClient.GetStream();
                while (Stream.CanWrite && messagesToSend.Count != 0)
                {
                    if(messagesToSend.TryDequeue(out byte[] message))
                    {
                        Stream.WriteAsync(message, 0, message.Length);
                        Stream.FlushAsync();
                    }
                    else
                    {
                        Debug.Log("Couldn't access queue");
                    }
                }                            
            }
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
        if (listener != null)
        {
            Debug.LogError($"listener already initialised, listening at {config.localIP}::{config.listenPort}");
            return;
        }
        listener = new TcpListener(IPAddress.Parse(config.localIP), config.listenPort);
        listener.Start();
        Debug.Log("listening...");
        TcpClient remoteClient = listener.AcceptTcpClient();
        if (remoteClient == null) Debug.Log("Failed to accept a client"); 
        recievedConnection?.Invoke();
        CreateClientThread(remoteClient);
    }
    #endregion
}
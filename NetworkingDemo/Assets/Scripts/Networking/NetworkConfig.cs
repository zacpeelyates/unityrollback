using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkConfig : MonoBehaviour
{
    [SerializeField]
    public int remotePort;
    [SerializeField]
    public int listenPort;
    [SerializeField]
    public string remoteIP;
    public string localIP;


    private void Reset()
    {
        localIP = FindLocalIP();
        remoteIP = localIP;
        remotePort = NetworkDefaults.REMOTE_PORT;
        listenPort = NetworkDefaults.LOCAL_PORT;
    }

    private void Awake()
    {
        localIP = FindLocalIP();

    }

    public void SwapPorts()
    {
        int temp = remotePort;
        remotePort = listenPort;
        listenPort = temp;
    }

    private string FindLocalIP()
    {
        foreach(IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if(ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;
    }
}

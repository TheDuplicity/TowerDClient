using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour
{
    public float serverTimeoutTimer;
    private float alivePingTimer;
    public bool connectedToServer;

    public static NetworkManager instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("instance already exists, destroying object.");
            Destroy(this);
        }
    }
    void Start()
    {
        serverTimeoutTimer = 0;
        alivePingTimer = 0;


    }

    // Update is called once per frame
    void Update()
    {
        //dont manage the network if connection is not active
        if (!Client.instance.tcp.active)
        {
            return;
        }
        serverTimeoutTimer += Time.deltaTime;
        if (serverTimeoutTimer > 5)
        {
            Debug.Log("Server connection timed out");
            serverTimeoutTimer = 0;
            Client.instance.disconnectFromServer();
            return;
        }
        alivePingTimer += Time.deltaTime;
        if (alivePingTimer > 1)
        {
            alivePingTimer = 0;
            ClientSend.ClientAlive(true);
        }
        IList readable, writeable;
        readable = new ArrayList();
        writeable = new ArrayList();

            readable.Add(Client.instance.tcp.socket.Client);
            writeable.Add(Client.instance.tcp.socket.Client);
            Socket.Select(readable, writeable, null, 100);

        if (readable.Count > 0)
        {
            Client.instance.tcp.readData();
        }
        if (writeable.Count > 0)
        {
           // Client.instance.tcp.readData();
        }

    }
}

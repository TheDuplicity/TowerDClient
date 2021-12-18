using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
{

    public static Client instance;
    //maximum size for ethernet
    public static int dataBufferSize = 1500;

    public string ip = "127.0.0.1";
    public int port = 5000;
    public int myId = 0;

    public TCP tcp;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

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

    private void Start()
    {
        tcp = new TCP();
        tcp.socket = null;
        initializeClientData();
    }

    public void ConnectToServer()
    {

        tcp = new TCP();
        tcp.socket = null;
        tcp.Connect();
      
    }

    public void disconnectFromServer()
    {
        //disconnect
        tcp.disconnect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;
        public bool active;

        public void Connect()
        {
            active = false;
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];

            socket.BeginConnect(instance.ip, instance.port , ConnectCallback, socket);
           
        }

        public void disconnect()
        {
            if (active)
            {
                active = false;
                Debug.Log("disconnected from server");
                //tell the server we're diconnecting
                ClientSend.ClientAlive(false);
                socket.GetStream().Close();
                socket.Close();
                socket = null;
            }


        }

        private void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);
            if (!socket.Connected)
            {

                Debug.Log("Couldnt connect to server.");

                socket.GetStream().Close();
                socket.Close();
                socket = null;

                return;
            }
            active = true;
            stream = socket.GetStream();

            receivedData = new Packet();

            readData();
        }

        public void readData()
        {
            
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (active)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"error sending data to server via TCP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {

                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    // disconnect
                    disconnect();
                    return;
                }
                byte[] data = new byte[byteLength];

                Array.Copy(receiveBuffer, data, byteLength);

                //handle data
                receivedData.Reset(HandleData(data));
               // stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            }
            catch (Exception ex)
            {
                Debug.Log($"Error receiving TCP data: {ex}");
                //disconnect client
                disconnect();
                return;
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {

                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        
                        packetHandlers[packetId](packet);
                    }
                });

                packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;

        }

    }

    private void initializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome},
            { (int)ServerPackets.serverAlive, ClientHandle.ServerAlive},
            { (int)ServerPackets.joinGameData, ClientHandle.JoinGameData},
            { (int)ServerPackets.timePing, ClientHandle.TimePing},
            { (int)ServerPackets.sendWorldUpdate, ClientHandle.handleWorldUpdate},
            { (int)ServerPackets.newPlayerJoined, }
        };
        Debug.Log("initialised packets");
    }

}

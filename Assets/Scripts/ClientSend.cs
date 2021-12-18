using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }
    #region Packets

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived)) 
        {
            packet.Write(Client.instance.myId);
            packet.Write(UIManager.instance.usernameField.text);
            SendTCPData(packet);
        }
    }

    public static void ClientAlive(bool alive)
    {
        using (Packet packet = new Packet((int)ClientPackets.clientAlive))
        {
            packet.Write(alive);
            SendTCPData(packet);
        }
    }

    public static void AttemptMinionCreation(bool inGame)
    {
  
        using (Packet packet = new Packet((int)ClientPackets.attemptMinionCreation))
        {
            packet.Write(inGame);
            SendTCPData(packet);
        }
    }
    //dont make the object, just tell the server the type and it will create and return the details to you
    public static void AttemptTowerCreation(Vector3 towerMousePos, bool inGame)
    {
        
        
        using (Packet packet = new Packet((int)ClientPackets.attemptTowerCreation))
        {
            packet.Write(towerMousePos.x);
            packet.Write(towerMousePos.y);
            packet.Write(towerMousePos.z);
            packet.Write(inGame);

            SendTCPData(packet);
        }
    }

    public static void SendMinionUpdate(GameManager.minionDefaultMessage message)
    {


        using (Packet packet = new Packet((int)ClientPackets.minionUpdate))
        {
            packet.Write(message.time);
            packet.Write(message.position.x);
            packet.Write(message.position.y);
            SendTCPData(packet);
        }
    }
    public static void SendTowerUpdate(GameManager.towerDefaultMessage message)
    {

        using (Packet packet = new Packet((int)ClientPackets.towerUpdate))
        {
            packet.Write(message.time);
            packet.Write(message.zRotation);
            SendTCPData(packet);
        }
    }

    public static void ShotBullet()
    {


        using (Packet packet = new Packet((int)ClientPackets.shotBullet))
        {
            packet.Write("bullet shot");
            SendTCPData(packet);
        }
    }


    public static void TimePing(int timerId)
    {
        NetworkManager.instance.serverTimeoutTimer = 0;
        //just need the id
        Debug.Log($"sending time ping to server on timer of id{timerId}");
        using (Packet packet = new Packet((int)ClientPackets.timePing))
        {
            packet.Write(timerId);
            SendTCPData(packet);
        }
    }


    #endregion
}

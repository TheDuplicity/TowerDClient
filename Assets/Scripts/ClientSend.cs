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

    public static void ChosePlayerType(int playerType)
    {
        // 0 is tower, 1 is minion
        using (Packet packet = new Packet((int)ClientPackets.chosePlayerType))
        {
            packet.Write(playerType);
            SendTCPData(packet);
        }
    }

    public static void TowerCreated(GameObject tower)
    {
        Tower towerDetails = tower.GetComponent<Tower>();
        // 0 is tower, 1 is minion
        using (Packet packet = new Packet((int)ClientPackets.chosePlayerType))
        {
            //
            //
            //
            SendTCPData(packet);
        }
    }

    #endregion
}

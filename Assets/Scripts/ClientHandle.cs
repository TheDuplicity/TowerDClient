﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"message from server; {msg}");
        Client.instance.myId = id;

        NetworkManager.instance.serverTimeoutTimer = 0;
        //send welcome received packet
        ClientSend.WelcomeReceived();
        //this just here to see the message getting received back
        //
        //
        /// delete soon
        UIManager.instance.startMenu.SetActive(false);
        UIManager.instance.serverMessageText.text = msg;
        UIManager.instance.afterServerResponse.SetActive(true);
    }

    public static void ServerAlive(Packet packet)
    {
        bool alive = packet.ReadBool();
        Debug.Log($"server alive? :- {alive}");
        if (alive)
        {
            NetworkManager.instance.serverTimeoutTimer = 0;
        }
        else
        {
            Client.instance.disconnectFromServer();
        }
    }

    public static void JoinGameData(Packet packet)
    {
        //go to next scene
        // fill in data into the temp data storage object to be passed into the level
        DataFromMenuToLevel dataStorage = FindObjectOfType<DataFromMenuToLevel>();
        dataStorage.playerSelectObjectType = packet.ReadInt();
        int numPlayers = packet.ReadInt();
        dataStorage.instantiateArrays(numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            dataStorage.types[i] = packet.ReadInt();
            dataStorage.ids[i] = packet.ReadInt();
            dataStorage.positions[i]= new Vector2(packet.ReadFloat(), packet.ReadFloat());
            dataStorage.zRotations[i] = packet.ReadFloat();
        }

        Debug.Log("got data from server, loading level");
        SceneManager.LoadScene(1);
    }

}

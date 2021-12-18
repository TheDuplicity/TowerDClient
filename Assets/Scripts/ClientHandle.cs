using System.Collections;
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
        //send 3 messages and time how long they take to come back and store in the network manager instance
        NetworkManager.instance.pingServerForRoundTripTime(3);
        
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
      //  Debug.Log($"server alive? :- {alive}");
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
        dataStorage.serverGameTime = packet.ReadFloat();
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

    public static void handleWorldUpdate(Packet packet)
    {
        float gameTime = packet.ReadFloat();
        if (GameManager.Instance.updateTimerWithOffsetTime)
        {
            GameManager.Instance.updateTimerWithOffsetTime = false;
            GameManager.Instance.updateTimerInGameLoop = true;
            GameManager.Instance.setGameTime( gameTime + GameManager.Instance.offsetTime);
        }
        GameManager.Instance.latestServerTime = gameTime;
        int minionScore = packet.ReadInt();
        int towerScore = packet.ReadInt();
        int numMinions = packet.ReadInt();
        GameManager.minionDefaultMessage[] minionMessages = new GameManager.minionDefaultMessage[numMinions];
        for (int i = 0; i < numMinions; i++)
        {
            minionMessages[i].clientId = packet.ReadInt();
            minionMessages[i].position = new Vector2(packet.ReadFloat(), packet.ReadFloat());
            minionMessages[i].time = gameTime;
        }       
        int numTowers = packet.ReadInt();
        GameManager.towerDefaultMessage[] towerMessages = new GameManager.towerDefaultMessage[numTowers];
        for (int i = 0; i < numTowers; i++)
        {
            towerMessages[i].clientId = packet.ReadInt();
            towerMessages[i].zRotation = packet.ReadFloat();
            towerMessages[i].time = gameTime;
        }
        GameManager.Instance.sendWorldUpdateToObjects(minionScore, towerScore, minionMessages, towerMessages);
    }

    public static void TimePing(Packet packet)
    {

        int timerID = packet.ReadInt();
       // Debug.Log($"received ping for timer id: {timerID}");
        //received response, stop timer and store it in network managers array
        NetworkManager.instance.addTimeToRoundTripTimesList(timerID);
    }

    public static void AddNewPlayer(Packet packet)
    {

        int id = packet.ReadInt();
        int type = packet.ReadInt();
        float zRot = packet.ReadFloat();
        Vector2 pos = new Vector2(packet.ReadFloat(), packet.ReadFloat());

        GameManager.Instance.CreateNewObject(id, type, pos, zRot);
    }
    public static void TowerShot(Packet packet)
    {

        int shootingTowerId = packet.ReadInt();
        GameManager.Instance.OtherTowerShot(shootingTowerId);
        // Debug.Log($"received ping for timer id: {timerID}");
        //received response, stop timer and store it in network managers array
        
    }
    public static void PlayerDied(Packet packet)
    {

        int deadPlayerId = packet.ReadInt();
        GameManager.Instance.KillObject(deadPlayerId);
        // Debug.Log($"received ping for timer id: {timerID}");
        //received response, stop timer and store it in network managers array

    }



}

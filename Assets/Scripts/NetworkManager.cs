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
    private Queue<float> roundTripTimes;
    private Dictionary<int, float> roundTripTimers;

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
        roundTripTimers = new Dictionary<int, float>();
        roundTripTimes = new Queue<float>();

    }

    // Update is called once per frame
    void Update()
    {
        //dont manage the network if connection is not active
        if (!Client.instance.tcp.active)
        {
            return;
        }
        // increment timers if weve sent out pings
        timePingTimers();

        serverTimeoutTimer += Time.deltaTime;
        if (serverTimeoutTimer > 6)
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
    private void timePingTimers()
    {
        if (roundTripTimers.Count > 0)
        {
            int currId = 0;
            int[] ids = new int[roundTripTimers.Count];
            foreach (KeyValuePair<int, float> item in roundTripTimers)
            {
                ids[currId] = item.Key;
                currId++;
            }
            for (int i = 0; i < roundTripTimers.Count; i++)
            {
                roundTripTimers[ids[i]] += Time.deltaTime;
            }
        }
    }
    public void addTimeToRoundTripTimesList(int timerId)
    {

            while (roundTripTimes.Count > 9)
            {
                roundTripTimes.Dequeue();
            }
            roundTripTimes.Enqueue(roundTripTimers[timerId]);
            roundTripTimers.Remove(timerId);

    }

    public float calculatenetWorkAverageHalfTripTime()
    {
        if (roundTripTimes.Count <= 0)
        {
            Debug.Log("tried to calculate half journey time with no times yet set");
            return -1;
        }
        float avgHalfJourneyTime = 0;
        foreach(float time in roundTripTimes)
        {
            avgHalfJourneyTime += time;
        }
        avgHalfJourneyTime = avgHalfJourneyTime / (float)roundTripTimes.Count;
        return avgHalfJourneyTime;
    }

    public void pingServerForRoundTripTime(int numPings)
    {
        if (roundTripTimers.Count <= 0)
        {
            addTimersDictionaryEmpty(numPings);

        }
        else
        {
            addTimersDictionaryHasTimers(numPings);
        }

    }

    private void addTimersDictionaryHasTimers(int numPings)
    {
        int[] currentTimerIds = new int[roundTripTimers.Count];
        int pos = -1;
        foreach (KeyValuePair<int, float> timer in roundTripTimers)
        {
            pos++;
            currentTimerIds[pos] = timer.Key;
            // do something with entry.Value or entry.Key
        }
        int setId = -1;
        for (int i = 0; i < numPings; i++)
        {
            bool noTimer = true;
            while (noTimer)
            {
                setId++;
                if (!doesTimerIdExist(currentTimerIds, setId))
                {
                    roundTripTimers.Add(setId, new float());
                    roundTripTimers[setId] = 0;
                    noTimer = false;
                    ClientSend.TimePing(setId);
                }

            }

        }
    }

    private void addTimersDictionaryEmpty(int numPings)
    {
 
        for (int i = 0; i < numPings; i++)
        {
            float setFloat = 0;
            roundTripTimers.Add(i, setFloat);          
            roundTripTimers[i] = 0;
            ClientSend.TimePing(i);                       
        }

    }
    
    private bool doesTimerIdExist(int[] ids, int id)
    {
        for (int j = 0; j < ids.Length; j++)
        {
            if (ids[j] == id)
            {
                return true;
            }
        }
        return false;
    }
}

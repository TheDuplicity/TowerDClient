using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour
{
    public bool playerControlled;
    private float maxMessages;
    private int id;
    public struct message
    {
        int id;
        Vector2 position;
        float rotation;
    }

    Queue<message> messages;
    // Start is called before the first frame update
    void Start()
    {
        playerControlled = false;
        maxMessages = 3;
        messages = new Queue<message>();
    }

    private void Update()
    {
        while (messages.Count > maxMessages)
        {
            messages.Dequeue();
        }
    }

    public void setId(int setId)
    {
        id = setId;
    }

    public int getId()
    {
        return id;
    }


    virtual public void handlePlayerControls() { }

}

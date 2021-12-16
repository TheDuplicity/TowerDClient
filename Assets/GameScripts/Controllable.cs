using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour
{
    public bool playerControlled;
    // Start is called before the first frame update
    void Start()
    {
        playerControlled = false;   
    }

    virtual public void handlePlayerControls() { }

}

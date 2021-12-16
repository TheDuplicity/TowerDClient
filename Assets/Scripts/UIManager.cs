using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool escMenuOpen;
    public static UIManager instance;

    public GameObject startMenu;
    public GameObject afterServerResponse;
    public GameObject escMenu;
    public InputField usernameField;
    public Button respawnButton;
    public Text serverMessageText;

    public Text TowerScoreText;
    public Text minionScoreText;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escMenuOpen = !escMenuOpen;
            escMenu.SetActive(escMenuOpen);
        }
    }

    public void ConnectToServer()
    {
      //  startMenu.SetActive(false);
      //  usernameField.interactable = false;
        Client.instance.ConnectToServer();

    }

    public void PingClosedStatus()
    {
        Client.instance.disconnectFromServer();
    }

    public void CloseStartMenu()
    {
        startMenu.SetActive(false);
    }

    public void deactivateRespawnButton()
    {
        respawnButton.gameObject.SetActive(false);
    }
    public void towerSelected()
    {
        DataFromMenuToLevel.instance.playerSelectObjectType = 0;
        ClientSend.ChosePlayerType(0);
        afterServerResponse.SetActive(false);
    }
    public void minionSelected()
    {
        DataFromMenuToLevel.instance.playerSelectObjectType = 1;
        ClientSend.ChosePlayerType(1);
        afterServerResponse.SetActive(false);
    }
}

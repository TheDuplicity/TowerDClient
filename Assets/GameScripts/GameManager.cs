using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public struct minionDefaultMessage
    {
        public int clientId;
        public Vector2 position;
        public float time;
    }
    public struct towerDefaultMessage
    {
        public int clientId;
        public float zRotation;
        public float time;
    }

    List<GameObject> minions;
    List<GameObject> towers;
    private GameObject tileSet;
     public GameObject minionPrefab;
     public GameObject towerPrefab;

    GameObject player;
    Vector3 PathStart;

    public int minionScore;
    public int towerScore;


    public float sendUpdateTimer;

    public float gameTime { get; private set; }
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(this);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {

        player = null;

        sendUpdateTimer = 0;

        minions = new List<GameObject>();
        towers = new List<GameObject>();
        tileSet = GameObject.Find("Tiles");

        PathStart = tileSet.GetComponent<CustomTileMap>().startTiles[0].transform.position;

        minionScore = 0;

        DataFromMenuToLevel instantiateLevelData = FindObjectOfType<DataFromMenuToLevel>();

        gameTime = instantiateLevelData.serverGameTime + NetworkManager.instance.calculatenetWorkAverageHalfTripTime();




        for (int i = 0; i < instantiateLevelData.numPlayers; i++)
        {
            GameObject newObject;

            if (instantiateLevelData.types[i] == 0)
            {
                newObject = addTower();

            } else if (instantiateLevelData.types[i] == 1)
            {
                newObject = addMinion();

            }
            else
            {                
                newObject = new GameObject();
            }
            //if the id of this player in the game is our id the this is us
            if (instantiateLevelData.ids[i] == Client.instance.myId)
            {
                setPlayer(newObject);
            }
            newObject.GetComponent<Controllable>().setType(instantiateLevelData.types[i]);
            newObject.transform.position = new Vector3(instantiateLevelData.positions[i].x, instantiateLevelData.positions[i].y, newObject.transform.position.z);
            newObject.transform.rotation = Quaternion.Euler(0, 0, instantiateLevelData.zRotations[i]);
            newObject.GetComponent<Controllable>().setId(instantiateLevelData.ids[i]);
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        sendUpdateTimer += Time.deltaTime;
        //send update every 50 ms
        if (sendUpdateTimer > 0.05)
        {
            sendUpdateTimer = 0;
            sendPlayerUpdate();

        }
        gameTime += Time.deltaTime;

        UIManager.instance.minionScoreText.text = minionScore.ToString();
        UIManager.instance.TowerScoreText.text = towerScore.ToString();
        UIManager.instance.gameTimeText.text = gameTime.ToString(".0#");


        if (Camera.main != null && player != null) {
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
        }
    }

    private void sendPlayerUpdate()
    {
        if (player.GetComponent<Controllable>().getType() == 0)
        {
            towerDefaultMessage message;
            message.time = gameTime;
            message.clientId = player.GetComponent<Controllable>().getId();
            message.zRotation = player.transform.rotation.eulerAngles.z;
            ClientSend.SendTowerUpdate(message);
        }
        else
        {
            minionDefaultMessage message;
            message.time = gameTime;
            message.clientId = player.GetComponent<Controllable>().getId();
            message.position = new Vector2(player.transform.position.x, player.transform.position.y);
            ClientSend.SendMinionUpdate(message);
        }
    }

    public void sendWorldUpdateToObjects(int setMinionScore, int setTowerScore, minionDefaultMessage[] minionMessages, towerDefaultMessage[] towerMessages)
    {
        minionScore = setMinionScore;
        towerScore = setTowerScore;
        for (int i = 0; i < minionMessages.Length; i++)
        {
            int id = minionMessages[i].clientId;
            GameObject selectedMinion = returnMinionWithThisClientId(id);
            if (selectedMinion == null)
            {
                continue;
            }
            selectedMinion.GetComponent<Minion>().AddMessage(minionMessages[i]);
        }
        for (int i = 0; i < towerMessages.Length; i++)
        {
            int id = towerMessages[i].clientId;
            GameObject selectedTower = returnTowerWithThisClientId(id);
            if (selectedTower == null)
            {
                continue;
            }
            selectedTower.GetComponent<Tower>().AddMessage(towerMessages[i]);
        }
    }
    private GameObject returnMinionWithThisClientId(int clientId)
    {
        foreach(GameObject minion in minions)
        {
            if (minion.GetComponent<Controllable>().getId() == clientId)
            {
                return minion;
            }
        }
        return null;
    }
    private GameObject returnTowerWithThisClientId(int clientId)
    {
        foreach (GameObject tower in towers)
        {
            if (tower.GetComponent<Controllable>().getId() == clientId)
            {
                return tower;
            }
        }
        return null;
    }
    private void setPlayer(GameObject newPlayer)
    {

        player = newPlayer;
        player.GetComponent<Controllable>().playerControlled = true;
    }
    public GameObject addMinion()
    {
        GameObject newMinion = Instantiate(minionPrefab);
        minions.Add(newMinion);
        return newMinion;
    }
    public GameObject addTower()
    {
        GameObject newTower = Instantiate(towerPrefab);
        towers.Add(newTower);
        return newTower;
    }

    public void spawnAsMinion()
    {
        GameObject minion = addMinion();
        setPlayer(minion);
    }

}

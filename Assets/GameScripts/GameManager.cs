using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<GameObject> minions;
    List<GameObject> towers;
    private GameObject tileSet;
     public GameObject minionPrefab;
     public GameObject towerPrefab;
    GameObject player;
    Vector3 PathStart;
    bool playerSelected;
    public int minionScore;
    public int towerScore;
    public bool playerIsTower;
    public bool playerPlacedTower;
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
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerPlacedTower = false;
        player = null;
        playerSelected = false;
        minions = new List<GameObject>();
        towers = new List<GameObject>();
        tileSet = GameObject.Find("Tiles");

        PathStart = tileSet.GetComponent<CustomTileMap>().startTiles[0].transform.position;

        minionScore = 0;

        DataFromMenuToLevel instantiateLevelData = FindObjectOfType<DataFromMenuToLevel>();

        if (instantiateLevelData.playerSelectObjectType == 0)
        {
            Debug.Log("player type is: tower");
            playerIsTower = true;
            
        } else if (instantiateLevelData.playerSelectObjectType == 1)
        {
            Debug.Log("player type is: minion");
            spawnAsMinion();
            playerIsTower = false;
        }

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
            newObject.transform.position = new Vector3(instantiateLevelData.positions[i].x, instantiateLevelData.positions[i].y, newObject.transform.position.z);
            newObject.transform.rotation = Quaternion.Euler(0, 0, instantiateLevelData.zRotations[i]);
            newObject.GetComponent<Controllable>().setId(instantiateLevelData.ids[i]);
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsTower)
        {
            if (!playerPlacedTower)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    spawnAsTower();
                }
            }
        }

        UIManager.instance.minionScoreText.text = minionScore.ToString();
        UIManager.instance.TowerScoreText.text = towerScore.ToString();
        /*
        if (!playerSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {

                spawnTower();
            }
            if (Input.GetMouseButtonDown(1))
            {

                spawnMinion();

            }
        }
        */

        if (Camera.main != null && player != null) {
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
        }
    }
    private void setPlayer(GameObject newPlayer)
    {
        playerSelected = true;
        player = newPlayer;
        player.GetComponent<Controllable>().playerControlled = true;
    }
    public GameObject addMinion()
    {
        GameObject newMinion = Instantiate(minionPrefab);
        newMinion.transform.position = PathStart;
        minions.Add(newMinion);
        return newMinion;
    }
    public GameObject addTower()
    {
        GameObject newTower = Instantiate(towerPrefab);
        towers.Add(newTower);
        return newTower;
    }

    public void spawnAsTower()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(mousePos, new Vector3(0, 0, 1), 100);
        if (hit) {
            GameObject hitObj = hit.transform.gameObject;
            if (hitObj.tag == "TowerTile")
            {
                Debug.Log("spawning tower");
                GameObject tower = addTower();
                player = tower;
                tower.transform.position = hit.transform.position;
                setPlayer(tower);
                playerPlacedTower = true;
            }

        }
    }

    public void spawnAsMinion()
    {
        GameObject minion = addMinion();
        setPlayer(minion);
    }

}

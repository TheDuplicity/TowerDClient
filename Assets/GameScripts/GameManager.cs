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
        player = null;
        playerSelected = false;
        minions = new List<GameObject>();
        towers = new List<GameObject>();
        tileSet = GameObject.Find("Tiles");
        PathStart = tileSet.GetComponent<CustomTileMap>().startTiles[0].transform.position;

        minionScore = 0;

    }

    // Update is called once per frame
    void Update()
    {

        UIManager.instance.minionScoreText.text = minionScore.ToString();
        UIManager.instance.TowerScoreText.text = towerScore.ToString();

        if (!playerSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {

                spawnTower();
                spawnMinion();
            }
            if (Input.GetMouseButtonDown(1))
            {

                spawnMinion();

            }
        }

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

    public void spawnTower()
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
            }

        }
    }

    public void spawnMinion()
    {
        GameObject minion = addMinion();
        setPlayer(minion);
    }

}

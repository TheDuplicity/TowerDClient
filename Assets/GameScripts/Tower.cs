using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Controllable
{
    public GameObject bulletPref;
    [SerializeField] private int maxBullets;
    [SerializeField] private float reloadSpeed;
    private float shootCooldown;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float wallSpawnRange;
    private float wallSpawnCooldown;
    private int numBullets;
    private float reloadTimer;
    // Start is called before the first frame update
    void Start()
    {

        numBullets = maxBullets;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControlled) {
            updateTimer(ref reloadTimer);
            updateTimer(ref wallSpawnCooldown);
            updateTimer(ref shootCooldown);
            handlePlayerControls();
        }
        else
        {
            updateFromSimulation();
        }
    }

    override public void handlePlayerControls()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            rotate(false);
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotate(true);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (reloadTimer <= 0)
            {
                
                if (numBullets > 0) {
                    if (shootCooldown <= 0) {
                        Shoot();
                    }
                }
                else
                {
                    Reload();
                }
            }
            else
            {
                Debug.Log($"still reloading for {reloadTimer} seconds.");
            }

        } else if (Input.GetMouseButtonDown(1))
        {
            if (wallSpawnCooldown <= 0) {
                SpawnWallAhead();
            }
        }
    }

    private void updateTimer(ref float timer)
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    private void updateFromSimulation()
    {


    }

    private void Shoot()
    {
        //spawn bullet with initial values
        shootCooldown = 0.35f;
        numBullets--;
        Debug.Log("shoot");

        //Bullet createBullet = bulletPref.GetComponent<Bullet>();
        GameObject createBullet = Instantiate(bulletPref);
        createBullet.GetComponent<Bullet>().createBulletData(6, 4, 25,transform.position, transform.rotation);
    }

    private void SpawnWallAhead()
    {
        //spawn wall at mouse position if in range of tower
        wallSpawnCooldown = 3.0f;
        Debug.Log("spawn wall");
    }

    private void Reload()
    {
        reloadTimer = 2.0f;
        numBullets = maxBullets;
        Debug.Log("reloading");
    }

    private void rotate(bool rotateClockwise)
    {
        if (rotateClockwise) {
            transform.rotation *= Quaternion.Euler(0,0,-90 * Time.deltaTime);
            Debug.Log("rotating clockwise");
        }
        else
        {
            transform.rotation *= Quaternion.Euler(0, 0, 90 * Time.deltaTime);
            Debug.Log("rotating anti-clockwise");
        }
    }

}

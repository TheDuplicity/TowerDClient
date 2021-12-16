﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minion : Controllable
{
    [SerializeField] private float speed;
    [SerializeField] private float health;
    // Start is called before the first frame update
    void Start()
    {
       // speed = 1;
       // health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControlled)
        {
            handlePlayerControls();

        }
        else
        {
            updateFromSimulation();
        }
    }

    override public void handlePlayerControls()
    {
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.W))
        {
            y++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            y--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x++;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x--;
        }
        Vector2 move = new Vector2(x, y);
        move.Normalize();
        move *= speed * Time.deltaTime;
        transform.position += new Vector3(move.x, move.y,0);
    }

    private void updateFromSimulation()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        GameObject colliderObj = collision.gameObject;
        Debug.Log("collided");
        if (colliderObj.tag == "Bullet")
        {
            Debug.Log(" collided with bullet");
            Bullet bullet = colliderObj.GetComponent<Bullet>();
            takeDamage(bullet.dealDamage());
            Destroy(colliderObj);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        GameObject colliderObj = collision.gameObject;
        if (colliderObj.tag == "TowerTile")
        {
            Debug.Log("inside tile");
            Vector3 colliderTowardMe = transform.position - colliderObj.transform.position;
            colliderTowardMe.Normalize();
            transform.position += colliderTowardMe * 0.03f * speed;
        }
        if (colliderObj.tag == "FinishTile")
        {
            //increment score
            GameManager.Instance.minionScore ++;
            die();
        }
    }

   

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.Instance.towerScore++;
            die();
        }
    }
    public void die()
    {
        Debug.Log("death");
        if (playerControlled) {
            Debug.Log("enabled button");
            UIManager.instance.respawnButton.gameObject.SetActive(true);

        }
        Destroy(gameObject);
    }

}

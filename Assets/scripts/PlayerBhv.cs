using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerBhv : MonoBehaviour
{
    Vector2[] path;
    Gamemanager manager;
    public SpriteRenderer renderer;
    Pathfinding pathfinder;
    public Sprite bobFront, bobBehind,bobLeft,bobRight,bobDed;

    float horizontal;
    float vertical;
    bool buttonUp = true;
    public bool ded = false;
    float moveX, moveY;
    bool notMoving;
    int pathIndex = -1;

    float speed = 0.02f;
    



    private void Awake()
    {
        manager = GameObject.FindObjectOfType<Gamemanager>();
        pathfinder = GameObject.FindObjectOfType<Pathfinding>();
    }


    void Update()
    {
        //graphics
        gameObject.GetComponent<LayerAdjustements>().AdjustLayer();
        UpdateSprite();

        if (!ded)
        {
            
            if (notMoving && Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 selectedTile = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
                GameObject tile = manager.getTile(selectedTile);
                // tile exists and moveable
                if (tile != null && tile.GetComponent<TileBhv>().move)
                {
                    
                    MoveTowards(selectedTile);
                }               
            }
                
                
                    

            

            // manual movement
            /*
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            UpdateSprite(horizontal, vertical);
            if (horizontal == 0 && vertical == 0)
                buttonUp = true;
            //MOVEMENT
            else if (notMoving && buttonUp)
            {
                
                // can move
                else if (getPlayerTile(horizontal, vertical).GetComponent<TileBhv>().move)
                {
                    //transform.position += new Vector3(horizontal, vertical);
                    moveX += horizontal;
                    moveY += vertical;
                    buttonUp = false;
                    manager.turn += 1;
                    
                }

            }
            else
                
            
        } */
            
            // actual movement




            notMoving = false;
            if (moveX != 0)
            {
                transform.position += new Vector3(speed * Mathf.Sign(moveX), 0);
                moveX -= speed * Mathf.Sign(moveX);
                moveX = Mathf.Round(moveX * 100) / 100;

            }
            if (moveY != 0)
            {
                transform.position += new Vector3(0, speed * Mathf.Sign(moveY));
                moveY -= speed * Mathf.Sign(moveY);
                moveY = Mathf.Round(moveY * 100) / 100;

            }
            else if (moveX == 0 && moveY == 0) // no movement
            {
                if (pathIndex != -1 && pathIndex <= path.Length - 1) // still movement to do
                {
                    moveX = path[pathIndex].x - transform.position.x;
                    moveY = path[pathIndex].y - transform.position.y;
                    pathIndex += 1;

                }
                else if (pathIndex != - 1)
                {
                    pathIndex = -1; // back to no movement state
                    notMoving = true;
                    manager.turn += 1;
                }
                else
                    notMoving = true;

            }


        }



    }

    public void MoveTowards(Vector2 position)
    {
        //
        
    }
    public void InvokeCheckIfDead()
    {
        Invoke("CheckIfDead",0.01f);
    }
   
    void Die(string cause)
    {
        // die stuff
        ded = true;
        Debug.Log("u ded");
        //animation
        gameObject.GetComponent<SpriteRenderer>().sprite = bobDed;
        //if(cause == "normal")
        //if(cause == "fall")
    }
    GameObject getTile(float x,float y)
    {       
        return manager.tileList[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
    }

    GameObject getPlayerTile(float offsetX = 0, float offsetY = 0)
    {
        return manager.tileList[Mathf.RoundToInt(transform.position.x + moveX + offsetX), Mathf.RoundToInt(transform.position.y + moveY + offsetY)];
    }

    void UpdateSprite()
    {
        SpriteRenderer rend = renderer;
        if (moveX > 0)
            rend.sprite = bobRight;
        else if(moveX < 0)
            rend.sprite = bobLeft;
        else if (moveY < 0)
            rend.sprite = bobFront;
        else if (moveY > 0)
            rend.sprite = bobBehind;
    }


}

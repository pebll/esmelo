using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBhv : MonoBehaviour
{
    
    Gamemanager manager;
    public SpriteRenderer indicator;
    SpriteRenderer renderer;
    //public SpriteRenderer addRenderer;
    //public Animator animator;
    
    public Sprite moveIndicator;
    public Sprite attackIndicator;
    [HideInInspector]
    public string tileName, obstacleName;
    public bool move = true;
    //public string die = "no";
    bool playerOnTile = false;
    float playerArrival = -10;
    [HideInInspector]
    public string status = "none";
    public Vector3 attackOrigin;
    // tilemaps
    [HideInInspector]
    public Tilemap baseTilemap, obstacleTilemap;
 
    // assets
    public RuleTile grassTile;
    public Sprite treeSprite;

    //paths
    string tilePath = "/Assets/tilemap/GroundTiles/";
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        manager = GameObject.FindObjectOfType<Gamemanager>();
        //player = GameObject.FindWithTag("Player");
    }
   
 
    public void Inherit(string tileName_, string obstacleName_, Tilemap basetilemap, Tilemap obstacletilemap)
    {
        baseTilemap = basetilemap;
        obstacleTilemap = obstacletilemap;
        tileName = tileName_;
        obstacleName = obstacleName_;
        if (obstacleName != "")
            move = false;
        manager.tileList[Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)] = gameObject;
        // set tile in tilemap base
        if(tileName != "")
        {
            var newTile = Resources.Load<RuleTile>("GroundTiles/" + tileName);
            baseTilemap.SetTile(Vector3Int.FloorToInt(transform.position), newTile);
        }

        if (obstacleName != "")
        {
            Sprite newSprite = Resources.Load<Sprite>("Obstacles/"+obstacleName);
            renderer.sprite = newSprite;
        }
        


    }

    private void Update()
    {
        UpdateStatus();
        //check characters
        foreach(GameObject character in manager.characters)
        {
            if (character.transform.position == transform.position)
            {
                if (playerArrival == -10)
                {
                    playerArrival = manager.turn;
                    

                }
                playerOnTile = true;
            }
            else
                playerOnTile = false;
        }
        
    }

    void UpdateStatus()
    {
        if (status == "none")
            indicator.sprite = null;
        else if (status == "move")
            indicator.sprite = moveIndicator;
        else if (status == "attack")
            indicator.sprite = attackIndicator;

    }
}

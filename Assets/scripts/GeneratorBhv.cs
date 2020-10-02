using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorBhv : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject TilePrfb;
    Gamemanager manager;
    public int sizeX = 10, sizeY = 10;
    public Tilemap baseTilemap, obstacleTilemap;
    public GameObject tileGrid;

    Tilemap baseTmp;
    Tilemap obstacleTmp;

    private void Awake()
    {
        manager = GameObject.FindObjectOfType<Gamemanager>();
        manager.mapSizeX = sizeX;
        manager.mapSizeY = sizeY;
    }
    void Start()
    {
        baseTmp = Instantiate(baseTilemap, new Vector3(-.5f, -.5f), Quaternion.identity);
        //obstacleTmp = Instantiate(obstacleTilemap, new Vector3(-.5f, -.5f), Quaternion.identity);
        baseTmp.transform.parent = tileGrid.transform;
        //obstacleTmp.transform.parent = tileGrid.transform;

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y< sizeY; y++)
            {
                GenerateTile(new Vector2(x, y));
            }
    }

    void GenerateTile(Vector2 pos)
    {
        string tileName;
        string obstacleName = "";
        float r = Random.value;
        //choose tile

        tileName = "Grass";
        if (r > 0.9f)
            obstacleName = "tree";
        
        if (pos.x == 0 || pos.x == sizeX -1 || pos.y == 0 || pos.y == sizeY -1)
            tileName = "";
        if (pos.x == 1 || pos.x == sizeX - 2)
            obstacleName = "";
        if(tileName != "")
        {   
            //generate tile
            GameObject tile = Instantiate(TilePrfb, pos, Quaternion.identity);
            tile.GetComponent<TileBhv>().Inherit(tileName, obstacleName,baseTmp, obstacleTmp);
            return;
             
        }        
    }
    


    //update for optimization
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerAdjustements : MonoBehaviour
{

    Gamemanager manager;
    SpriteRenderer renderer;
    public SpriteRenderer renderer2;
    public SpriteRenderer renderer3;
    public SpriteRenderer renderer4;
    public bool isPlayer;

    private void Awake()
    {
        manager = GameObject.FindObjectOfType<Gamemanager>();
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        AdjustLayer();
    }
    public void AdjustLayer()
    {
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = (manager.mapSizeY - Mathf.RoundToInt(transform.position.y)) * 5;
        if(renderer2)
            renderer2.sortingOrder = (manager.mapSizeY - Mathf.RoundToInt(transform.position.y)) * 5 + 1;
        if (renderer3)
            renderer3.sortingOrder = (manager.mapSizeY - Mathf.RoundToInt(transform.position.y)) * 5 + 2;
        if (renderer4)
            renderer4.sortingOrder = (manager.mapSizeY - Mathf.RoundToInt(transform.position.y)) * 5 + 3;

    }

}

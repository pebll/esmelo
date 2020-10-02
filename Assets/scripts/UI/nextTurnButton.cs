using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class nextTurnButton : MonoBehaviour
{
    Gamemanager manager;
    public GameObject b;
    private void Awake()
    {
        manager = FindObjectOfType<Gamemanager>();
    }
    void Update()
    {
        /*
        if (!manager.isEnemyTurn)
        {
            b.SetActive(true);
            if (manager.IsTurnFinished(true))
                b.GetComponent<Image>().color = Color.green;
            else
                b.GetComponent<Image>().color = Color.yellow;
        }   
        else
            b.SetActive(false);
            */

    }       
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBhv : MonoBehaviour
{
    public GameObject healthSlotPfb;
    int maxHealth = 4;
    int actualHP;

    GameObject[] healthSlots;
    void Start()
    {
        actualHP = maxHealth;
        // create new bar
        for(int i = 0; i < maxHealth; i++)
        {
            Vector3 pos = getSlotPos(i);
            GameObject slot = Instantiate(healthSlotPfb, pos, Quaternion.identity);
            slot.transform.parent = transform;
        }
    }

    Vector3 getSlotPos(int index)
    {
        float x = transform.position.x + (index * 50f);
        float y = transform.position.y;
        return new Vector3(x, y, transform.position.z);
    }
    
    void Update()
    {
        // if(hp != actualHP)
        //     update the bar
    }
}

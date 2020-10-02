using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlotBhv : MonoBehaviour
{
    public bool isActive;
    Image renderer;
    public Color OnColor;
    public Color OffColor;
    bool lastActiveState;

    private void Awake()
    {
        renderer = transform.GetChild(0).GetComponent<Image>();
        isActive = true;
        lastActiveState = isActive;
    }
    private void Update()
    {
        if(isActive && !lastActiveState)
        {
            lastActiveState = true;
            renderer.color = OnColor;
        }
        else if(!isActive && lastActiveState)
        {
            lastActiveState = false;
            renderer.color = OffColor;
        }
            
    }
}

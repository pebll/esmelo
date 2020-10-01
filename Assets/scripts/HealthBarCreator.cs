using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarCreator : MonoBehaviour
{
    public CharacterBhv character;
    public Image slot;
    int lastHP;
    public bool inCanvasSpace = false;
    Image[] slots;

    private void Start()
    {
        slots = new Image[character.baseHealth];
        lastHP = character.health;
        for(int i = 0; i < character.baseHealth; i++)
        {
            float slotHeight = slot.GetComponent<RectTransform>().rect.height;
            Vector3 pos = new Vector3(0, -i * .09375f, 0);
            if (inCanvasSpace)
                pos *= 200;
            Image newSlot = Instantiate(slot, transform.position, Quaternion.identity);
            newSlot.transform.parent = transform;
            newSlot.transform.position += pos;
            newSlot.transform.localScale = new Vector3(.25f, .25f, 1);
            slots[i] = newSlot;
        }
    }

    private void Update()
    {
        if(character.health != lastHP)
        {
            // update HP bar
            lastHP = character.health;
            for (int i = 0; i < character.baseHealth; i++)
            {
                if (i >= character.baseHealth - lastHP)
                    slots[i].GetComponent<HealthSlotBhv>().isActive = true;
                else
                    slots[i].GetComponent<HealthSlotBhv>().isActive = false;
            }
        }
    }
}

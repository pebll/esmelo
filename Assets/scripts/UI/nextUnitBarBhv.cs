using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nextUnitBarBhv : MonoBehaviour
{
    public GameObject[] slots;
    Gamemanager manager;
    public Color red, blue;
    private void Awake()
    {
        manager = FindObjectOfType<Gamemanager>();
    }

    public void UpdateBar()
    {
        if (true) {

            for (int i = 0; i < 5; i++)
            {
                CharacterBhv c = null;
                int turn = -1;
                // get the corresponding character
                if (manager.playOrderIndex + i < manager.PlayOrder.Count)
                    c = manager.PlayOrder[manager.playOrderIndex + i];

                else if (manager.playOrderIndex + i == manager.PlayOrder.Count) // next turn
                    turn = manager.turn + 1;

                else if (-1+manager.playOrderIndex + i - manager.PlayOrder.Count < manager.PlayOrder.Count)
                    c = manager.PlayOrder[-1+manager.playOrderIndex + i - manager.PlayOrder.Count];

                else if (manager.playOrderIndex + i - manager.PlayOrder.Count == manager.PlayOrder.Count) // next turn
                    turn = manager.turn + 2;

                else if (-1+manager.playOrderIndex + i - manager.PlayOrder.Count * 2 < manager.PlayOrder.Count)
                    c = manager.PlayOrder[-1+manager.playOrderIndex + i - manager.PlayOrder.Count * 2];
                // special cases
                else if (manager.playOrderIndex + i - manager.PlayOrder.Count * 3 < manager.PlayOrder.Count)
                    c = manager.PlayOrder[manager.playOrderIndex + i - manager.PlayOrder.Count * 3];
                else if (manager.playOrderIndex + i - manager.PlayOrder.Count * 4 < manager.PlayOrder.Count)
                    c = manager.PlayOrder[manager.playOrderIndex + i - manager.PlayOrder.Count * 4];
                else
                    c = manager.PlayOrder[manager.playOrderIndex + i - manager.PlayOrder.Count * 5];

                // turn display
                if (turn != -1)
                {
                    slots[i].transform.GetChild(0).gameObject.SetActive(false);
                    slots[i].transform.GetChild(1).gameObject.SetActive(false);
                    slots[i].transform.GetChild(2).gameObject.SetActive(false);
                    slots[i].transform.GetChild(3).gameObject.SetActive(true);
                    slots[i].transform.GetChild(4).gameObject.SetActive(true);
                    slots[i].transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = turn.ToString();
                    continue;

                }
                else
                {
                    slots[i].transform.GetChild(0).gameObject.SetActive(true);
                    slots[i].transform.GetChild(1).gameObject.SetActive(true);
                    slots[i].transform.GetChild(2).gameObject.SetActive(true);
                    slots[i].transform.GetChild(3).gameObject.SetActive(false);
                    slots[i].transform.GetChild(4).gameObject.SetActive(false);
                }
                    
                
                // change background color
                if (!c.isEnemy)
                    slots[i].transform.GetChild(0).GetComponent<Image>().color = blue;
                else
                    slots[i].transform.GetChild(0).GetComponent<Image>().color = red;
                // change sprite
                slots[i].transform.GetChild(1).GetComponent<Image>().sprite = c.renderer.sprite;
            }
        }
        
    }
}

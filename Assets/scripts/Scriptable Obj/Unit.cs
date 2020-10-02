using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnit", menuName = "Unit")]
public class Unit : ScriptableObject
{
    public new string name;

    public Sprite sprite;

    // stats
    public int health = 4;
    public int armor = 0;
    public int damage = 1;
    public int MP = 3;
    [Range(0,10)]
    public int initiative = 5;
    public int range = 1;

    public float CalculateSP()
    {
        float SP = 0;
        SP += health * 1.5f;
        SP += armor * 4;
        SP += damage * 5;
        SP += (range - 1) * 5; // if ranged
        SP += initiative * 0.2f;
        SP += MP * 2;
        return SP;
    }


}

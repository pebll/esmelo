using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarStep
{
    public Vector2 position;
    public int gScore;
    public int hScore;
    public AstarStep parent;
    public int fScore;

    public void Init(Vector2 pos)
    {
        position = pos;
        gScore = 0;
        hScore = 0;
        parent = null;
        updateFscore();
    }

    public void updateFscore()
    {
        fScore = gScore + hScore;
    }
}

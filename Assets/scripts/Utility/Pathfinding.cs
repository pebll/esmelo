using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

using JetBrains.Annotations;
using Object = UnityEngine.Object;

public class Pathfinding : MonoBehaviour
{
    List<AstarStep> openList = new List<AstarStep>();
    List<AstarStep> closedList = new List<AstarStep>();
    Gamemanager manager;
    List<Vector2> attackableTiles = new List<Vector2>();
    bool allyYes;
    private void Awake()
    {
        manager = GameObject.FindObjectOfType<Gamemanager>();
    }

    private void Start()
    {
        //Invoke("Test",0.1f);
    }
    
    public Vector2[] FindShortestPath(Vector2 start, Vector2 goal, bool fromAlly)
    {
        allyYes = fromAlly;
        bool foundPath = false;

        List<Vector2> goals = new List<Vector2>();
        
        if (manager.getCharacter(goal))
        {
            Debug.Log("want to reach character");
            if(manager.getTile(goal + new Vector2(1, 0))!= null && manager.getTile(goal + new Vector2(1, 0)).GetComponent<TileBhv>().move)
                goals.Add(goal + new Vector2(1,0));
            if (manager.getTile(goal + new Vector2(-1, 0)) != null && manager.getTile(goal + new Vector2(-1, 0)).GetComponent<TileBhv>().move)
                goals.Add(goal + new Vector2(-1, 0));
            if (manager.getTile(goal + new Vector2(0, 1)) != null && manager.getTile(goal + new Vector2(0, 1)).GetComponent<TileBhv>().move)
                goals.Add(goal + new Vector2(0, 1));
            if (manager.getTile(goal + new Vector2(0, -1)) != null && manager.getTile(goal + new Vector2(0, -1)).GetComponent<TileBhv>().move)
                goals.Add(goal + new Vector2(0, -1));
            Debug.Log(goals.Count);
        }

        if (start == goal)
        {
            Debug.LogError("already there");
            return null;
        }
        if (manager.getTile(goal)== null || manager.getTile(goal).GetComponent<TileBhv>().move == false)
        {

            Debug.LogError("unwalkable terrain at" + goal);
            return null;
        }
        openList = new List<AstarStep>();
        closedList = new List<AstarStep>();

        Vector2[] path = null;
        AstarStep currentStep;
        AstarStep tempStep;
        List<Vector2> adjacentSteps;
        insertInOpenList(createStep(start));
        
        // - - - - -
        do
        {
            SortOpenList();
            currentStep = openList[0];
            //Debug.Log("current step position: " + currentStep.position);

           
            closedList.Add(currentStep);
            openList.RemoveAt(0);

            // check if angekommen
            if(currentStep.position == goal || goals.Contains(currentStep.position))
            {
                
                foundPath = true;
                tempStep = currentStep;

                Debug.Log("Path found: ");
                // construct array
                int pathLength = getPathLength(tempStep);
                path = new Vector2[pathLength];
                int i = pathLength - 1;
                do
                {
                    
                    path[i] = tempStep.position;
                    tempStep = tempStep.parent;
                    i -= 1;
                }
                while (tempStep.parent != null);
                
                

            }
            //get adjacent tiles
            adjacentSteps = getAdjacentTiles(currentStep.position);
            foreach(Vector2 stepPos in adjacentSteps)
            {
                AstarStep step = createStep(stepPos);
                if (ContainsStep(closedList, stepPos))//(closedList.Contains(step))
                    continue;
                if (ContainsStep(openList, stepPos) == false)//(!openList.Contains(step)) // is not in open list
                {
                    step.parent = currentStep;
                    step.gScore = currentStep.gScore + 1;
                    step.hScore = getHscore(step.position, goal);
                    step.updateFscore();
                    openList.Add(step);
                }
                else // is already in open list
                {
                    SortOpenList();
                }
            }
        }
        // - - - - - 
        while (openList.Count > 0);

        if (!foundPath)
        {
            Debug.Log("no path found");
            
        }
        return path;



    }

    public List<Vector2> GetMoveableTiles(Vector2 start, int moves, bool fromAlly = true)
    {
        allyYes = fromAlly;
        openList = new List<AstarStep>();
        closedList = new List<AstarStep>();

        List<Vector2> moveableTiles = new List<Vector2>();
        AstarStep currentStep;
        AstarStep tempStep;
        List<Vector2> adjacentSteps;
        insertInOpenList(createStep(start));

        // - - - - -
        do
        {
            SortOpenList();
            currentStep = openList[0];
            if (currentStep.position != start && !moveableTiles.Contains(currentStep.position))
                moveableTiles.Add(currentStep.position);
            closedList.Add(currentStep);
            openList.RemoveAt(0);

            
            //get adjacent tiles
            adjacentSteps = getAdjacentTiles(currentStep.position);
            foreach (Vector2 stepPos in adjacentSteps)
            {
                
                AstarStep step = createStep(stepPos);
                if (currentStep.gScore + 1 > moves)
                    continue;

                if (ContainsStep(closedList, stepPos))//(closedList.Contains(step))
                    continue;
                if (ContainsStep(openList, stepPos) == false)//(!openList.Contains(step)) // is not in open list
                {
                    step.parent = currentStep;
                    step.gScore = currentStep.gScore + 1;
                    step.hScore = getHscore(step.position, start);
                    step.updateFscore();
                    openList.Add(step);
                }
                else // is already in open list
                {
                    SortOpenList();
                }
            }
        }
        // - - - - - 
        while (openList.Count > 0);
        /*Debug.Log("moveable tiles from " + start + " with " + moves + "moves:");
        foreach (Vector2 p in moveableTiles)
            Debug.Log(p);*/
        return moveableTiles;
    }
    int getPathLength(AstarStep tempStep)
    {
        int i = 0;
        do
        {
            i += 1;
            tempStep = tempStep.parent;
        }
        while (tempStep.parent != null);
        return i;
    }
    void SortOpenList()
    {
        

        //DebugList(openList, "openList before sorting");
        openList = openList.OrderBy(e => e.fScore).ToList();
        //DebugList(openList, "openList after sorting");

    }
    void DebugList(List<AstarStep> list, string name  = "list")
    {
        Debug.Log("Debugging " + name);
        foreach (AstarStep s in list)
            Debug.Log("position: " + s.position + ", fScore: " + s.fScore);
    }
    bool ContainsStep(List<AstarStep> list, Vector2 pos)
    {
        foreach(AstarStep step in list)
        {
            if (step.position == pos)
                return true;
        }
        return false;

    }
    public AstarStep findStepInOpenList(Vector2 pos)
    {
        foreach (AstarStep step in openList)
            if (step.position == pos)
                return step;
        Debug.LogError("step not found");
        return null;
    }
    public void insertInOpenList(AstarStep step)
    {
        openList.Add(step);
        
        
    }

    public int getHscore(Vector2 coord1, Vector2 coord2)
    {
        return Mathf.RoundToInt(Mathf.Abs(coord2.x - coord1.x) + Mathf.Abs(coord2.y - coord1.y));
    }

    public AstarStep createStep(Vector2 pos)
    {
        AstarStep a = new AstarStep();
        a.Init(pos);
        return a;
    }

    public List<Vector2> getAdjacentTiles(Vector2 tileCoord)
    {
        List<Vector2> list = new List<Vector2>();

        float x = tileCoord.x;
        float y = tileCoord.y;
        /*
        if (manager.getTile(new Vector2(x + 1, y)) != null && manager.getTile(new Vector2(x + 1, y)).GetComponent<TileBhv>().move == true)// && manager.getCharacter(new Vector3(x+1,y)) == null)
            list.Add(new Vector2(x + 1, y));
        if (manager.getTile(new Vector2(x - 1, y)) != null && manager.getTile(new Vector2(x - 1, y)).GetComponent<TileBhv>().move == true)// && manager.getCharacter(new Vector3(x-1,y)) == null)
            list.Add(new Vector2(x - 1, y));
        if (manager.getTile(new Vector2(x, y + 1)) != null && manager.getTile(new Vector2(x, y + 1)).GetComponent<TileBhv>().move == true)// && manager.getCharacter(new Vector3(x,y+1)) == null)
            list.Add(new Vector2(x, y + 1));
        if (manager.getTile(new Vector2(x , y -1)) != null && manager.getTile(new Vector2(x, y - 1)).GetComponent<TileBhv>().move == true)// && manager.getCharacter(new Vector3(x,y-1)) == null)
            list.Add(new Vector2(x, y-1));
            
            */
        if(CanGoTo(new Vector3(x+1,y)))
            list.Add(new Vector2(x+1, y));
        if (CanGoTo(new Vector3(x-1, y)))
            list.Add(new Vector2(x-1, y));
        if (CanGoTo(new Vector3(x, y+1)))
            list.Add(new Vector2(x, y+1));
        if (CanGoTo(new Vector3(x, y-1)))
            list.Add(new Vector2(x, y-1));

        return list;
    }
    
    bool CanGoTo(Vector3 pos)
    {

        if (manager.getTile(pos) != null && manager.getTile(pos).GetComponent<TileBhv>().move == true)
            if (manager.getCharacter(pos) == null)
                return true;
            else
            {
                bool isenemey = manager.getCharacter(pos).GetComponent<CharacterBhv>().isEnemy;
                if (!isenemey && allyYes || isenemey && !allyYes)
                    return true;
                
            }
        return false;
    }
   

    
    bool isInsideMap(float x, float y)
    {
        if (x > manager.mapSizeX || x < 0 || y < 0 || y > manager.mapSizeY)
        {
            Debug.Log("the coord " + x + " - " + y + " is not in map");
            return false;
        }
            
        return true;
    }



    



}

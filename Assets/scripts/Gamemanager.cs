using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public class Gamemanager : MonoBehaviour
{
    [HideInInspector]
    public List<CharacterBhv> PlayOrder;
    GameObject UICanvas;
    public GameObject nextUnitBar;

    public GameObject characterPrefab;
    public GameObject[,] tileList;
    //public GameObject player;
    public List<GameObject> characters;
    [HideInInspector]
    public int turn = 0, playOrderIndex = 0;
    public bool isEnemyTurn = false;

    public int mapSizeX = 10, mapSizeY = 10;
    public float unitSwapDelay = 1.2f;

    Pathfinding pathfinder;
    GameObject selectedCharacter = null; 
    CharacterBhv playingCharacter = null;

    // new generation 
    public Unit[] allyUnitPool;
    public Unit[] enemyUnitPool;
    public int baseAllySP = 50;
    public int baseEnemySP = 40;

    public class AttackTile
    {
        public Vector2 fromPos, targetPos; 
    }
    private void Awake()
    {
        //GenerateUnits();
        GameObject[] cObj = GameObject.FindGameObjectsWithTag("character");
        foreach (GameObject c in cObj)
        {
            characters.Add(c);
            PlayOrder.Add(c.GetComponent<CharacterBhv>());
        }
            
        pathfinder = GameObject.FindObjectOfType<Pathfinding>();
        tileList = new GameObject[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
            for (int y = 0; y < mapSizeY; y++)
            {
                tileList[x, y] = null;
            }
        //
        
        PlayOrder = PlayOrder.OrderByDescending(w => w.initiative).ToList();
        
    }

    private void Start()
    {
        NextTurn();
    }
    private void Update()
    {
        UICanvas = GameObject.FindWithTag("UICanvas");
        // tiles show 
        if (selectedCharacter != null && selectedCharacter.GetComponent<CharacterBhv>().notMoving)
            GetSurroundingTiles(selectedCharacter);
        else
        {
            DeStatusAllTiles();
        }




        // check for enemy turn finish
        /*if (isEnemyTurn)
            if (IsTurnFinished(false))
                PlayerTurn();*/
        if (playingCharacter != null)
            if (IsTurnFinished(playingCharacter.gameObject))
            {
                nextUnit();               
            }
                
        

        // check for clicks
        if (Input.GetMouseButtonDown(0) && !isEnemyTurn)// clicked with select button
        {
            //DeselectAllCharacters();
            
            // check for tile
            GameObject selectedTile = getTileAtMouse();
          
            //check for characters
            //SelectCharacter(getCharacterAtMouse());
        }

        if (Input.GetMouseButtonDown(1) && !isEnemyTurn) // clicked with action button
        {
            GameObject selectedTile = getTileAtMouse();
            GameObject targetCharacter = getCharacterAtMouse();
            if(selectedCharacter != null)
            {
                if (selectedTile != null)
                    // MOVE
                    if (selectedTile.GetComponent<TileBhv>().status == "move")
                    {
                        
                        selectedCharacter.GetComponent<CharacterBhv>().MoveTowards(selectedTile.transform.position);
                    }
                    // ATTACK
                    else if (selectedTile.GetComponent<TileBhv>().status == "attack")
                    {
                        selectedCharacter.GetComponent<CharacterBhv>().Attack(targetCharacter);
                        
                    }
            }          
        }   
    }

    public void SpawnUnit(Unit unit, Vector3 pos, bool isEnemy = false)
    {
        GameObject newC = Instantiate(characterPrefab, pos, Quaternion.identity);
        newC.GetComponent<CharacterBhv>().isEnemy = isEnemy;
        newC.GetComponent<CharacterBhv>().Inherit(unit.name);

    }
    void GenerateUnits()
    {
        float allySP = baseAllySP, enemySP = baseEnemySP;
        for(int i = 0; i < 10; i++) // for ally
        {
            List<Unit> pool = GetAvailableUnits(allyUnitPool, allySP);
            if (pool.Count == 0)
                break;
            Unit newUnit = pool[Random.Range(0, pool.Count - 1)];
            Vector3 pos = new Vector3(Random.Range(1, mapSizeX - 1), 1, 1);
            SpawnUnit(newUnit, pos);
            allySP -= newUnit.CalculateSP();
        }
        for (int i = 0; i < 10; i++) // for enemy
        {
            List<Unit> pool = GetAvailableUnits(enemyUnitPool, enemySP);
            if (pool.Count == 0)
                break;
            Unit newUnit = pool[Random.Range(0, pool.Count - 1)];
            Vector3 pos = new Vector3(Random.Range(1, mapSizeX - 1), mapSizeY-2, 1);
            SpawnUnit(newUnit, pos, true);
            enemySP -= newUnit.CalculateSP();
        }
    }

    List<Unit> GetAvailableUnits(Unit[] pool, float SP)
    {
        List<Unit> units = new List<Unit>();
        foreach(Unit u in pool)
        {
            
            if (u.CalculateSP() < SP)
            {
                units.Add(u);
            }  
        }
        Debug.Log("count"+units.Count);
        return units;
    }
    public void EnemyTurn()
    {
        isEnemyTurn = true;
        DeselectAllCharacters();
        
        RefreshCharacters(false);
        // screen info : "Enemy turn"

        foreach (GameObject character in characters)
            if (character.GetComponent<CharacterBhv>().isEnemy)
            {
                CharacterBhv enemy = character.GetComponent<CharacterBhv>();
                enemy.turnState = 0;
                //enemy.MoveTowards(getNearestCharacter(character.transform.position).transform.position);
            }
                
    }
    
    public void nextUnit()
    {
        Debug.Log("next unit: " + (playOrderIndex + 1));
        UpdatePlayOrder();

        DeselectAllCharacters();
        playOrderIndex += 1;
        if (playOrderIndex >= PlayOrder.Count)
        {
            NextTurn();
        }
        playingCharacter = PlayOrder[playOrderIndex];
        if (playingCharacter == null || playingCharacter.ded)
            nextUnit();
        // refresh unit
        RefreshCharacter(playingCharacter);
        //focus on unit
        Camera.main.GetComponent<CameraBhv>().Focus(playingCharacter.gameObject.transform.position, unitSwapDelay);
        nextUnitBar.GetComponent<nextUnitBarBhv>().UpdateBar();
        
        Invoke("nextUnitReal", unitSwapDelay * 2/4);
    }
    public void nextUnitReal()
    {
        if (playingCharacter.ded)
        {
            nextUnit();
            return;
        }
            
        // select unit
        if (playingCharacter.isEnemy)
        {
            playingCharacter.turnState = 0;
        }
        else // is player character
        {
            SelectCharacter(playingCharacter.gameObject);
            //selectedCharacter = playingCharacter.gameObject;
        }
    }
    public void PlayerTurn()
    {
        isEnemyTurn = false;
        DeselectAllCharacters();
        

        // screen info : "Your turn"

        // reinitialisation valeurs
        turn += 1;
        RefreshCharacters(true);

    }
    public void NextTurn()
    {
        Debug.Log("Turn: " + (turn + 1));
        turn += 1;
        playOrderIndex = -1;

        
        nextUnit();

    }
    
    public void UpdatePlayOrder()
    {
        CharacterBhv activeCharacter = null;
        if (playOrderIndex != -1)
            activeCharacter = PlayOrder[playOrderIndex];
        PlayOrder.Clear();
        /*GameObject[] cObj = GameObject.FindGameObjectsWithTag("character");
        foreach (GameObject c in cObj)
        {
            PlayOrder.Add(c.GetComponent<CharacterBhv>());
        }*/
        foreach (GameObject cObj in characters)
            PlayOrder.Add(cObj.GetComponent<CharacterBhv>());
        PlayOrder = PlayOrder.OrderByDescending(w => w.initiative).ToList();

        if (playOrderIndex != -1)
            for (int i = 0; i < PlayOrder.Count; i++)
                if (PlayOrder[i] == activeCharacter)
                    playOrderIndex = i;


        if(playOrderIndex != -1)
            nextUnitBar.GetComponent<nextUnitBarBhv>().UpdateBar();
    }
    public void CheckBattleWinner() // "ally" / "enemy"
    {
        bool allyAlive = false;
        bool enemyAlive = false;
        foreach(GameObject character in characters)
        {
            if (character.GetComponent<CharacterBhv>().isEnemy && !character.GetComponent<CharacterBhv>().ded)
                enemyAlive = true;
            else if (!character.GetComponent<CharacterBhv>().isEnemy && !character.GetComponent<CharacterBhv>().ded)
                allyAlive = true;
        }
        if (allyAlive == false)
        {
            //pause
            UICanvas.transform.GetChild(0).gameObject.SetActive(true);
        }
        if (enemyAlive == false)
        {
            //pause
            UICanvas.transform.GetChild(1).gameObject.SetActive(true);
        }
            
        
    }
    public List<AttackTile> getAttackTiles(GameObject character)
    {
        List<AttackTile> attackTiles = new List<AttackTile>();
        List<Vector2> moveableTiles = pathfinder.GetMoveableTiles(character.transform.position, character.GetComponent<CharacterBhv>().MP);
        moveableTiles.Add(character.transform.position);
        foreach(Vector3 tile in moveableTiles)
        {
            if (getCharacter(tile) && tile != character.transform.position)
            {
                continue;
            }
            foreach(GameObject c in characters)
            {
                CharacterBhv enemy = c.GetComponent<CharacterBhv>();
                if(enemy.isEnemy != character.GetComponent<CharacterBhv>().isEnemy) // not same team
                {
                    if(CanAttack(tile, c.transform.position, character))
                    {
                        AttackTile aT = new AttackTile();
                        aT.targetPos = enemy.transform.position;
                        aT.fromPos = tile;
                        attackTiles.Add(aT);
                    }
                }
            }
                
        }
        //Debug.Log("can attack " + attackTiles.Count);
        return attackTiles;
    }
    public bool CanAttack(Vector3 attackerPos, Vector3 target, GameObject attackerObj)
    {
        CharacterBhv attacker = attackerObj.GetComponent<CharacterBhv>();
        Vector2  moveDistVec= attackerPos - attackerObj.transform.position;
        Vector2 distVec = attackerPos - target;
        float dist = Mathf.Round(Mathf.Abs(distVec.x) + Mathf.Abs(distVec.y));
        float moveDist = Mathf.Round(Mathf.Abs(moveDistVec.x) + Mathf.Abs(moveDistVec.y));
        
        if (dist <= attacker.attackRange && (attacker.MP - moveDist ) >= 0)//attacker.attackCost)
        {
            return true;
        }
            
        return false;
    }
    public GameObject getNearestCharacter(Vector3 position, bool allyFaction = true)
    {
        GameObject nearestC = null;
        float minDist = 10000;
        foreach(GameObject character in characters)
        {
            CharacterBhv c = character.GetComponent<CharacterBhv>();
            if (allyFaction && !c.isEnemy || !allyFaction && c.isEnemy)
            {
                Vector2 distVec = position - character.transform.position;
                float dist = Mathf.Abs(distVec.x) + Mathf.Abs(distVec.y);
                if(dist < minDist)
                {
                    minDist = dist;
                    nearestC = character;
                }
            }
        }
        return nearestC;
    }
    void RefreshCharacters(bool fromAlly)
    {
        //characters = GameObject.FindGameObjectsWithTag("character");
        foreach (GameObject character in characters)
        {
            CharacterBhv c = character.GetComponent<CharacterBhv>();
            if(fromAlly && !c.isEnemy || !fromAlly && c.isEnemy)
            {
                c.MP = c.baseMP;
                if (c.isEnemy)
                    c.turnState = -1;
                // check damage (fall / spikes...)
            }

        }
    }
    void RefreshCharacter(CharacterBhv c)
    {
        c.MP = c.baseMP;
        c.AP = 1;
        c.turnState = -1;
    }
    public bool IsTurnFinished(GameObject character)
    {
        
        CharacterBhv c  = character.GetComponent<CharacterBhv>();
        if (!c.isEnemy)
            if (c.AP > 0 || !c.notMoving)
                return false;
        if (c.isEnemy)
            if (c.turnState != -2)
                return false;
        // make with turn state
        // check character actions
        /*
        foreach (GameObject character in characters)
        {
            CharacterBhv 
                c = character.GetComponent<CharacterBhv>();
            if (!c.isEnemy && fromAlly)
                if (c.MP > 0)
                    return false;
            if (c.isEnemy && !fromAlly)
                if (c.turnState != -2)
                    return false;

        }*/
        return true;
            

    }
    void SelectCharacter(GameObject character)
    {
        if (character == null)
            return;

        DeselectAllCharacters();
        
        selectedCharacter = character;
        selectedCharacter.GetComponent<CharacterBhv>().selected = true;
        
        
    }
    public void GetSurroundingTiles(GameObject character)
    {
        DeStatusAllTiles();
        if(!isEnemyTurn)
            if(character != null  &&  (!character.GetComponent<CharacterBhv>().isEnemy))
            {
                //List<Vector2> moveableTiles = pathfinder.GetMoveableTiles(selectedCharacter.transform.position, selectedCharacter.GetComponent<CharacterBhv>().MP);
                List<Vector2> moveableTiles = pathfinder.GetMoveableTiles(character.transform.position, character.GetComponent<CharacterBhv>().MP);
                List<AttackTile> attackableTiles = getAttackTiles(character);
                foreach (Vector2 pos in moveableTiles)
                {
                    if(getCharacter(pos) == null)
                        getTile(pos).GetComponent<TileBhv>().status = "move";
                }
                foreach( AttackTile aT in attackableTiles)
                {
                    getTile(aT.targetPos).GetComponent<TileBhv>().status = "attack";
                    getTile(aT.targetPos).GetComponent<TileBhv>().attackOrigin = aT.fromPos;
                }
                

            }
        

    }
    void DeselectAllCharacters()
    {
        selectedCharacter = null;
        foreach (GameObject c in characters)
            c.GetComponent<CharacterBhv>().selected = false;
    }

    public void DeStatusAllTiles()
    {
        foreach (GameObject t in tileList)
            if(t != null)
                     t.GetComponent<TileBhv>().status = "none";
    }
    

    public GameObject getTile(Vector2 pos)
    {
        float x = Mathf.RoundToInt(pos.x);
        float y = Mathf.RoundToInt(pos.y);
        if (x >= mapSizeX || x < 0 || y < 0 || y >= mapSizeY)
            return null;
        return tileList[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
    }

    GameObject getTileAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 selectedTile = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
        GameObject tile = getTile(selectedTile);
        // tile exists
        if (tile != null)
            return tile;
        return null;
    }
    public GameObject getCharacter(Vector3 pos)
    {
        foreach(GameObject c in characters)
        {
            if (c.transform.position == pos)
                return c;
        }
        return null;
    }

    public GameObject getCharacterAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 selectedPos = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
        foreach (GameObject c in characters)
        {
            if (c.transform.position == selectedPos)
                return c;
        }
        return null;
    }

    void Restart()
    {
        Debug.Log("restart");
        SceneManager.LoadScene(0);
    }

    void debug()
    {
        Debug.Log("turn: " + turn);
    }
}



using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class CharacterBhv : MonoBehaviour
{
    public GameObject worldCanvas;
    public Slider[] healthSliders;
    Vector2[] path;
    Gamemanager manager;
    public GameObject CharacterBar;
    public GameObject characterCanvas;
    public SpriteRenderer renderer;
    public GameObject selectedIndicator;
    public GameObject damageIndicator;
    Pathfinding pathfinder;
    //public Sprite front, bobBehind, bobLeft, bobRight, bobDed;
    
    Vector3 moveGoal = new Vector3(-100, -100);
    GameObject attackTarget;

    float horizontal;
    float vertical;
    bool buttonUp = true;
    public bool ded = false;
    float moveX, moveY;
    [HideInInspector]
    public bool notMoving;
    int pathIndex = -1;

    float speed = 0.02f;

    public bool selected = false;
    [HideInInspector]
    public int turnState = -1; // -2 = turn finished -1: turn available 0: initialisation 1: move 2: attack 3: finish 
    GameObject afterMoveAttackTarget = null;
    // base stats
    [HideInInspector]
    public float SP;
    [HideInInspector]
    public int baseHealth, initiative, baseMP, baseArmor, baseDamage, attackRange;
    
    public bool isEnemy = false;
    
    int attackCost = 1;
    

    public string unitName = "default";
    Unit unit;

    // game stats
    [HideInInspector]
    public int MP, health, armor, damage, AP;

    private void Awake()
    {
        manager = FindObjectOfType<Gamemanager>();
        pathfinder = FindObjectOfType<Pathfinding>();
        /*unit = Resources.Load<Unit>("Units/" + unitName);
        // base values ( derived from scriptable obj)
        initiative = unit.initiative;
        baseMP = unit.MP;
        baseHealth = unit.health;
        baseArmor = unit.armor;
        baseDamage = unit.damage;
        attackRange = unit.range;
        renderer.sprite = unit.sprite;

        // game values
        MP = baseMP;
        AP = 1;
        health = baseHealth;
        armor = baseArmor;
        damage = baseDamage;
        // other stuff
        if (isEnemy)
            characterCanvas.SetActive(false);
        else
            InitialiseBar();

        SP = unit.CalculateSP();
        Debug.Log("SP of " + unitName + " : " + SP);*/
        Inherit(unitName);

    }

    public void Inherit(string name)
    {
        unitName = name;
        //Debug.Log("inheriting from " + name);
        unit = Resources.Load<Unit>("Units/" + name);
        // base values ( derived from scriptable obj)
        initiative = unit.initiative;
        baseMP = unit.MP;
        baseHealth = unit.health;
        baseArmor = unit.armor;
        baseDamage = unit.damage;
        attackRange = unit.range;
        renderer.sprite = unit.sprite;

        // game values
        MP = baseMP;
        AP = 1;
        health = baseHealth;
        armor = baseArmor;
        damage = baseDamage;
        // other stuff
        if (isEnemy)
            characterCanvas.SetActive(false);
        else
            InitialiseBar();

        SP = unit.CalculateSP();
        Debug.Log("SP of " + unitName + " : " + SP);
    }

    void Update()
    {
        if (isEnemy)
            AI();
        //graphics
        gameObject.GetComponent<LayerAdjustements>().AdjustLayer();
        UpdateSprite();

        if (!ded)
        {

            if (notMoving && Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 selectedTile = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
                GameObject tile = manager.getTile(selectedTile);
                // tile exists and moveable
                if (tile != null && tile.GetComponent<TileBhv>().move)
                {

                    //MoveTowards(selectedTile);
                }
            }
            
            //CheckIfDead();
            // actual movement

            notMoving = false;
            if (moveX != 0)
            {
                transform.position += new Vector3(speed * Mathf.Sign(moveX), 0);
                moveX -= speed * Mathf.Sign(moveX);
                moveX = Mathf.Round(moveX * 100) / 100;

            }
            if (moveY != 0)
            {
                transform.position += new Vector3(0, speed * Mathf.Sign(moveY));
                moveY -= speed * Mathf.Sign(moveY);
                moveY = Mathf.Round(moveY * 100) / 100;

            }
            else if (moveX == 0 && moveY == 0 && !isEnemy)
                if (pathIndex != -1 && pathIndex <= path.Length - 1 && MP > 0)
                {

                    moveX = path[pathIndex].x - transform.position.x;
                    moveY = path[pathIndex].y - transform.position.y;
                    pathIndex += 1;
                    MP -= 1;

                }
                else if (pathIndex != -1)// || MP == 0 && isEnemy)
                {
                    manager.GetSurroundingTiles(gameObject);
                    pathIndex = -1; // back to no movement state
                    notMoving = true;
                    if(afterMoveAttackTarget!= null)
                    {
                        Attack(afterMoveAttackTarget);
                        afterMoveAttackTarget = null;
                    }

                }
                else
                    notMoving = true;


        }
    }

    // -2 = turn finished -1: turn available 0: plan move 1: move 2: attack 3: finish 
    void AI()
    {
        if (!ded)
        {
            if (turnState == -2 || turnState == -1)
                return;
            else if (turnState == 0) // programming actions (core)
            {
                attackTarget = null;
                // target finding
                Vector2[] path = null;
                GameObject target = null;
                foreach (GameObject character in manager.characters)
                {
                    if(!character.GetComponent<CharacterBhv>().isEnemy)
                    {
                        Vector2[] tempPath = pathfinder.FindShortestPath(transform.position, character.transform.position, true);
                        if (tempPath == null)
                            continue;
                        if (path == null)
                        {
                            path = tempPath;
                            target = character;
                        }  
                        else if (tempPath.Length < path.Length)
                        {
                            path = tempPath;
                            target = character;
                        }
                    }
                }             
                //path = pathfinder.FindShortestPath(transform.position, target.transform.position, true);
                int i = 0;
                
                if (target == null && path == null)
                {
                    Debug.Log("cannot reach any character");
                    turnState = 2;
                }

                foreach (Vector3 pos in path)
                {
                    i++;
                    if (manager.CanAttack(pos, target.transform.position, gameObject))// target in  range
                    {
                        Debug.Log("can attack");
                        attackTarget = target;
                        moveGoal = pos;
                        break;
                    }

                    if (i >= MP)
                    {
                        moveGoal = pos;
                        break;
                    }

                }
                if (manager.CanAttack(transform.position, target.transform.position, gameObject))
                    moveGoal = transform.position;
                if (moveGoal == new Vector3(-100, -100))
                    moveGoal = path[path.Length - 1];

                Debug.Log("drztgydhujikol" + path.Length);

                // next state
                MoveTowards(moveGoal);

                turnState = 1;
            }
            else if (turnState == 1) // actual moving
            {
                // stands on a tile
                if (moveX == 0 && moveY == 0) // no movement
                {
                    if (transform.position != moveGoal) // not arrived yet
                    {
                        moveX = path[pathIndex].x - transform.position.x;
                        moveY = path[pathIndex].y - transform.position.y;
                        pathIndex += 1;
                        MP -= 1;
                    }
                    else if (transform.position == moveGoal)// arrived at goal
                    {
                        pathIndex = -1; // back to no movement state
                        notMoving = true;
                        turnState = 2;
                    }

                    /*if (pathIndex != -1 && pathIndex <= path.Length - 1 && MP > 0)
                    {

                        moveX = path[pathIndex].x - transform.position.x;
                        moveY = path[pathIndex].y - transform.position.y;
                        pathIndex += 1;
                        MP -= 1;

                    }
                    else if (pathIndex != -1 || MP == 0 && isEnemy)
                    {
                        if (!isEnemy)
                            manager.GetSurroundingTiles(gameObject);
                        pathIndex = -1; // back to no movement state
                        notMoving = true;

                    }
                    else
                        notMoving = true;*/

                }
            }
            else if (turnState == 2)// attack
            {
                if (AP > 0 && attackTarget != null)
                {                 
                    Attack(attackTarget);
                }

                turnState = -2;
            }
        }
    }

    void TakeDamage(int damage, Vector2 hitdirection)
    {
        health -= damage;
        CheckIfDead();
        GameObject g = Instantiate(damageIndicator, transform.position, Quaternion.identity);
        g.transform.parent = worldCanvas.transform;
        g.GetComponent<DamageIndicatorBhv>().damage = damage;
        g.GetComponent<DamageIndicatorBhv>().hitDirection = hitdirection;
    }
    public void Attack(GameObject target)
    {
        CharacterBhv targetBhv = target.GetComponent<CharacterBhv>();
        if (!isEnemy && manager.getTile(target.transform.position).GetComponent<TileBhv>().attackOrigin != transform.position)
        {
            MoveTowards(manager.getTile(target.transform.position).GetComponent<TileBhv>().attackOrigin);
            afterMoveAttackTarget = target;
            
        }

        else if (manager.CanAttack(transform.position, target.transform.position, gameObject))
        {
            AP -= 1;
            // basic damage
            int dam = damage;
            Vector3 dir = target.transform.position - transform.position;
            targetBhv.TakeDamage(dam, dir);
            // anim
            
            dir *= 0.4f;
            Vector3 startPos = transform.position;

            LeanTween.move(gameObject, transform.position + dir, 0.05f).setEaseInSine();
            LeanTween.move(gameObject, startPos, 0.3f).setEaseInSine().setDelay(0.05f);

            
            // counter
        }
        else
            Debug.LogError("cant attack " + target.name + "because of not in range or not enougth MP");

    }
    public void MoveTowards(Vector2 position)
    {
        path = pathfinder.FindShortestPath(transform.position, position, !isEnemy);
        if (path != null)
        {
            pathIndex = 0;
        }
            
        else
            Debug.LogError("cant move at " + position);

    }
    
   
    public void PassTurn()
    {       
        MP = 0;
        AP = 0;
    }
    public void CheckIfDead()
    {
        //CHECK IF DED
        if (health <= 0)
            Die();
    }
    void Die()
    {
        // die stuff
        ded = true;
        Debug.Log("u ded");
        //animation
        LeanTween.alpha(gameObject, 0, 1);
        manager.characters.Remove(gameObject);
        manager.UpdatePlayOrder();
        Invoke("Kill", 1);

        manager.CheckBattleWinner();
    }
    void Kill()
    {
        
        Destroy(gameObject);
    }
    GameObject getTile(float x, float y)
    {
        return manager.tileList[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
    }

    GameObject getPlayerTile(float offsetX = 0, float offsetY = 0)
    {
        return manager.tileList[Mathf.RoundToInt(transform.position.x + moveX + offsetX), Mathf.RoundToInt(transform.position.y + moveY + offsetY)];
    }

    void InitialiseBar()
    { 
        CharacterBar.transform.GetChild(1).GetComponent<Text>().text = unitName;
        CharacterBar.transform.GetChild(2).GetComponent<Image>().sprite = renderer.sprite;

    }
    void UpdateSprite()
    {

        // make with animator instead
        /*
        SpriteRenderer rend = renderer;
        if (moveX > 0)
            rend.sprite = bobRight;
        else if (moveX < 0)
            rend.sprite = bobLeft;
        else if (moveY < 0)
            rend.sprite = bobFront;
        else if (moveY > 0)
            rend.sprite = bobBehind;*/

        // 
        
        // if selected stuff
        if (!isEnemy)
        {

            // selected 
            if (selected)
            {
                // show selected indicator
                selectedIndicator.SetActive(true);
                characterCanvas.SetActive(true);

                // show selected dings 
            }
            else
            {
                // dont show selected indicator
                selectedIndicator.SetActive(false);
                characterCanvas.SetActive(false);


            }
            if(!notMoving)
                selectedIndicator.SetActive(false);
        }
        
    }


}


// old shit


// manual movement
/*
horizontal = Input.GetAxisRaw("Horizontal");
vertical = Input.GetAxisRaw("Vertical");
UpdateSprite(horizontal, vertical);
if (horizontal == 0 && vertical == 0)
    buttonUp = true;
//MOVEMENT
else if (notMoving && buttonUp)
{

    // can move
    else if (getPlayerTile(horizontal, vertical).GetComponent<TileBhv>().move)
    {
        //transform.position += new Vector3(horizontal, vertical);
        moveX += horizontal;
        moveY += vertical;
        buttonUp = false;
        manager.turn += 1;

    }

}
else


} */

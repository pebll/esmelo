using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorBhv : MonoBehaviour
{
    public Vector2 hitDirection;
    public int damage;
    public float destroyTime = 2f;
    float alpha;
    Vector3 offset = new Vector3(0, .8f, 0);
    Vector3[] path;
    Vector3 startPos;
    float xMovement = 1f;
    private void Start()
    {
        transform.position += offset;
        transform.localScale = new Vector3(0f, 0f,0);
        path = new Vector3[4];
        startPos = transform.position;
        path[0] = transform.position;
        path[1] = startPos;
        path[2] = new Vector3(hitDirection.x*xMovement/4, 0.4f+hitDirection.y/6,1) + startPos;
        path[3] = new Vector3(hitDirection.x * xMovement/2, -0.5f, 1)+startPos;
        ShowDamage();
    }
    public void ShowDamage()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = damage.ToString();
        // alpha
        LeanTween.value(gameObject,1, 0, 0.6f).setDelay(1.4f).setEaseOutCubic().setOnUpdate((float val) => {
            GetComponent<TMPro.TextMeshProUGUI>().alpha = val;
        });
        //size up
        LeanTween.value(gameObject, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), .8f).setEaseOutElastic().setOnUpdate((Vector3 val) => {
            transform.localScale = val;
        });
        //size down
        LeanTween.value(gameObject, new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, .2f), .8f).setEaseOutExpo().setDelay(0.6f).setOnUpdate((Vector3 val) => {
            transform.localScale = val;
        });
        //move up
        
        LeanTween.value(gameObject, path[1], path[2], .3f).setEaseOutCubic().setOnUpdate((Vector3 val) => {
            transform.position = val;
        });
        
        LeanTween.value(gameObject, path[2], path[3], .4f).setEaseInCubic().setDelay(.3f).setOnUpdate((Vector3 val) => {
            transform.position = val;
        });


        Destroy(gameObject, destroyTime);
    }

  

}

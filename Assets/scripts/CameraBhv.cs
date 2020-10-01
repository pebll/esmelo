
using UnityEngine;

public class CameraBhv : MonoBehaviour
{
    Gamemanager manager;

    Vector3 mouseStartPos;
    Vector3 cameraStartPos;
    
    float speed = 10;
    float maxZoom = 2;
    float minZoom = 5;

    float unitSwapDelay = .2f;
    private void Awake()
    {
        manager = FindObjectOfType<Gamemanager>();
    }
    private void Update()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        // movement
        if (Input.GetMouseButtonDown(2)) // pressed
        {
            mouseStartPos = mousePos();
            cameraStartPos = transform.position;
        }
        if (Input.GetMouseButton(2)) // held down
        {
            transform.position = cameraStartPos + (mouseStartPos - mousePos());
        }
        transform.position = new Vector3(Mathf.Max(transform.position.x, 0), Mathf.Max(transform.position.y, 0), transform.position.z);
        transform.position = new Vector3(Mathf.Min(transform.position.x, manager.mapSizeX), Mathf.Min(transform.position.y, manager.mapSizeY), transform.position.z);
        //zoom
        float zoomAmount = 3;
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && inZoomLimit(true)) // forward
        {
            Camera.main.orthographicSize-=zoomAmount / 10;
            
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0&& inZoomLimit(false)) // backwards
        {
            Camera.main.orthographicSize+=zoomAmount / 10;
        }
    }

    bool inZoomLimit(bool zooming)
    {
        if (zooming)
            return (Camera.main.orthographicSize > maxZoom);
        else
            return (Camera.main.orthographicSize < minZoom);

    }

    public void Focus(Vector3 pos, float time)
    {
        Debug.Log("focus");
        Vector3 newpos = new Vector3(pos.x, pos.y, -10);
        LeanTween.move(gameObject, newpos, time - unitSwapDelay).setEase(LeanTweenType.easeOutCirc).setDelay(unitSwapDelay);
    }
    Vector3 mousePos()
    {
        return Input.mousePosition *speed /1000;
    }

}

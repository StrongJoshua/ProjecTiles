using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public Camera cam;
    public int maxZoom;
    public int minZoom;
    public GameObject highlight;
    public int x, y;
    public Text coordinates;
    public MapGenerator map;
    public int step;
    public SelectedHighlight selector;
    public float tweenTime;

    private int xDel, yDel;
    private float delay, lastTime;

    private const float defaultDelay = .2f;

    void Start()
    {
        delay = defaultDelay;
        lastTime = 0;
        x = 0;
        y = 0;
        xDel = 0;
        yDel = 0;
    }

    void OnPreRender()
    {
        selector.curTileX = x;
        selector.curTileY = y;
    }

    // Update is called once per frame
    void Update()
    {
        xDel = 0;
        yDel = 0;

        if (Input.GetKey(KeyCode.UpArrow))
            yDel += 1;
        if (Input.GetKey(KeyCode.DownArrow))
            yDel -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            xDel += 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            xDel -= 1;

        moveHighlight();
        coordinates.text = map.GetTileType(x, y) + "";
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.forward) * 6, 0.3f);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.back) * 6, 0.3f);

        }

    }

    private void moveHighlight()
    {
        if (Time.timeSinceLevelLoad - lastTime > delay)
        {
            if (xDel == 0 && yDel == 0)
            {
                lastTime = 0;
                delay = defaultDelay;
            } else {
                x += xDel;
                y += yDel;
                x = Mathf.Max(Mathf.Min(x, map.SizeX - 1), 0);
                y = Mathf.Max(Mathf.Min(y, map.SizeY - 1), 0);
                highlight.transform.position = new Vector3(x * step, 0, y * step);

                if (delay > .1f)
                    delay -= .04f;
                lastTime = Time.timeSinceLevelLoad;
            }
        }

        iTween.MoveUpdate(this.gameObject, highlight.transform.position + new Vector3(0, 20f, -45f), tweenTime);
    }
}

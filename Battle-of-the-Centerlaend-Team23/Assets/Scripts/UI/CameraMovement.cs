using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
/// <summary>
/// Handles the camera movement and zoom.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    protected Camera camera;
    protected float speed = 0.001f;
    protected float speedZoom = 250f;

    public virtual void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        move();
        scroll();
    }

    protected virtual void move()
    {
        Vector2 mov = new Vector2(0, 0);
        if (Input.GetKey("s"))
        {
            mov.y -= 1;
        }

        if (Input.GetKey("w"))
        {
            mov.y += 1;
        }

        if (Input.GetKey("a"))
        {
            mov.x -= 1;
        }

        if (Input.GetKey("d"))
        {
            mov.x += 1;
        }
        mov.Normalize();
        transform.Translate(mov * (speed * camera.orthographicSize * 2));
    }

    private void scroll()
    {
        camera.orthographicSize =
            Mathf.Clamp((camera.orthographicSize + Input.mouseScrollDelta.y * Time.deltaTime * speedZoom * -1), 1, 100); 
    }
}

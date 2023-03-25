using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
        float Sheight = Screen.height / 1920f;
        float Swidth = Screen.width / 1080f;
        camera.orthographicSize = 5 * (Sheight / Swidth);
    }
}

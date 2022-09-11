using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatio : MonoBehaviour
{
    public float x = 16.0f;
    public float y = 9.0f;
    public Camera camera;

    public float screenHeight;
    public float screenWidth;

    void Start()
    {
        camera = GetComponent<Camera>();
        UpdateAspectRatio();
    }

    private void Update()
    {
        if (screenHeight != Screen.height || screenWidth != Screen.width)
        {
            UpdateAspectRatio();
        }
    }

    void UpdateAspectRatio()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        float scaleheight = (screenWidth / screenHeight) / (x / y);
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}

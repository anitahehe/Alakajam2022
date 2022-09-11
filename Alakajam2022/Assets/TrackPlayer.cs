using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    public GameObject player;
    public BoxCollider2D mapBounds;

    private float xMin, xMax, yMin, yMax;
    private float camY,camX;
    private float camOrthsize;
    private float cameraRatio;
    private Vector3 smoothPos;
    public float smoothSpeed = 0.5f;

    void Start()
    {
        xMin = mapBounds.bounds.min.x-50.0f;
        xMax = mapBounds.bounds.max.x+50.0f;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;
        var mainCam = GetComponent<Camera>();
        camOrthsize = mainCam.orthographicSize;
        cameraRatio = (xMax + camOrthsize) / 2.0f;
    }
    void Update()
    {
        camY = Mathf.Clamp(player.transform.position.y, yMin + camOrthsize, yMax - camOrthsize);
        camX = Mathf.Clamp(player.transform.position.x, xMin + cameraRatio, xMax - cameraRatio);
        smoothPos = Vector3.Lerp(this.transform.position, new Vector3(camX, camY, this.transform.position.z), smoothSpeed);
        this.transform.position = smoothPos;
    }
}

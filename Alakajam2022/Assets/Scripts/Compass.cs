using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;

    public enum ISLAND { HELM, NAVI, SKUL }
    public ISLAND island;

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (island == ISLAND.SKUL)
        {
            sr.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void LateUpdate()
    {
        // Direction
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation,rotation,speed *Time.deltaTime);

        // Appearance
        switch (island)
        {
        case ISLAND.HELM:
            sr.enabled = !GameManager.Instance().hasShipwrightItem;
            break;
        case ISLAND.NAVI:
            sr.enabled = !GameManager.Instance().hasNavigatorItem;
            break;
        case ISLAND.SKUL:
            sr.enabled = GameManager.Instance().hasNavigatorItem && GameManager.Instance().hasShipwrightItem;
            break;
        }
    }
}

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
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        gm = GameManager.Instance();
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
            sr.enabled = !gm.hasShipwrightItem && gm.hasMetCaptain;
            break;
        case ISLAND.NAVI:
            sr.enabled = !gm.hasNavigatorItem && gm.hasMetCaptain;
            break;
        case ISLAND.SKUL:
            sr.enabled = gm.hasNavigatorItem && gm.hasShipwrightItem;
            break;
        }
    }
}

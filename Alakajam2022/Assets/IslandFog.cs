using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandFog : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance().hasNavigatorItem && GameManager.Instance().hasShipwrightItem)
        {
            Destroy(this.gameObject);
        }
    }
}

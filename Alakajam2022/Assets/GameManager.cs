using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool hasMetCaptain = false;
    public bool hasNavigatorItem = false;
    public bool hasShipwrightItem = false;

    private static GameManager inst;

    private void Awake()
    {
        inst = this;
    }

    public static GameManager Instance()
    {
        return inst;
    }
}

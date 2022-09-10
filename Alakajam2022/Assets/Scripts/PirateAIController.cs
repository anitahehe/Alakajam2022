using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 1. Player must be in sightrange
 * 2. Player must be in danger zone
 */


public class PirateAIController : BoatController
{
    public Transform target;

    public float sightRange;
    public bool sightCheck = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update() 
    { 
       
    }
}

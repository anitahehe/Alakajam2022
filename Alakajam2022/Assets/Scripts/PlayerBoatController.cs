using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoatController : BoatController
{
    void Update()
    {
        if (Input.GetKey("w"))
        {
            forward = true;
            backwards = false;
        }
        else if (Input.GetKey("s"))
        {
            backwards = true;
            forward = false;
        }
        else
        {
            backwards = false;
            forward = false;
        }

        if (Input.GetKey("a"))
        {
            left = true;
            right = false;
        }
        else if (Input.GetKey("d"))
        {
            left = false;
            right = true;
        }
        else
        {
            left = false;
            right = false;
        }
    }
}

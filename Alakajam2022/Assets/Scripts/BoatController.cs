using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float maxSpeed;
    public float forwardThrust;
    public float reverseThrust;
    public float deceleration;
    public float torque;
    
    private Rigidbody rb;

    protected bool forward = false;
    protected bool backwards = false;
    protected bool left = false;
    protected bool right = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
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

    void FixedUpdate()
    {
        // Apply forward/reverse thrust and hand-applied deceleration.
        if (forward)
        {
            rb.AddForce(transform.up * forwardThrust);
        } 
        if (backwards)
        {
            rb.AddForce(-transform.up * reverseThrust);
        } 
        if(!forward && !backwards)
        {
            rb.velocity -= deceleration * rb.velocity;
        }
        // Apply torque based turning.
        Quaternion lastRotation = transform.rotation;
        if ((left && forward) || (right && backwards))
        {
            rb.AddRelativeTorque(-transform.forward * torque);
        } 
        if ((right && forward) || (left && backwards))
        {
            rb.AddRelativeTorque(transform.forward * torque);
        }
        Quaternion currentRotation = transform.rotation;
        Quaternion relativeRotation = currentRotation * Quaternion.Inverse(lastRotation);
        rb.velocity = relativeRotation * rb.velocity;
        // Don't exceed maximum speed.
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }
}

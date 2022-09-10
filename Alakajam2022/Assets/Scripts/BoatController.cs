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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Apply forward/reverse thrust and hand-applied deceleration.
        if (Input.GetKey("w"))
        {
            rb.AddForce(transform.up * forwardThrust);
        } else if (Input.GetKey("s"))
        {
            rb.AddForce(-transform.up * reverseThrust);
        } else
        {
            rb.velocity -= deceleration * rb.velocity;
        }
        // Apply torque based turning.
        Quaternion lastRotation = transform.rotation;
        if (Input.GetKey("a"))
        {
            rb.AddRelativeTorque(-transform.forward * torque);
        } else if (Input.GetKey("d"))
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

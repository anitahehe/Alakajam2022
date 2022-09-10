using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    public float maxSpeed;
    public float forwardThrust;
    public float reverseThrust;
    public float deceleration;
    public float torque;

    private Rigidbody rb;

    public BoatController boatController;

    private void Start()
    {
        boatController = GetComponent<BoatController>();
        if (boatController == null)
        {
            Debug.LogError("Error: Boat Movement does not have attacked Boat Controller");
        }
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Apply forward/reverse thrust and hand-applied deceleration.
        if (boatController.forward)
        {
            rb.AddForce(transform.up * forwardThrust);
        }
        if (boatController.backwards)
        {
            rb.AddForce(-transform.up * reverseThrust);
        }
        if (!boatController.forward && !boatController.backwards)
        {
            rb.velocity -= deceleration * rb.velocity;
        }
        // Apply torque based turning.
        Quaternion lastRotation = transform.rotation;
        if ((boatController.left && boatController.forward) || (boatController.right && boatController.backwards))
        {
            rb.AddRelativeTorque(-transform.forward * torque);
        }
        if ((boatController.right && boatController.forward) || (boatController.left && boatController.backwards))
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

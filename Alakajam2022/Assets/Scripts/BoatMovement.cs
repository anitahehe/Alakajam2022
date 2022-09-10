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

    private Rigidbody2D rb;

    public BoatController boatController;

    private void Start()
    {
        boatController = GetComponent<BoatController>();
        if (boatController == null)
        {
            Debug.LogError("Error: Boat Movement does not have attacked Boat Controller");
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Apply forward/reverse thrust and hand-applied deceleration.
        if (boatController.forward)
        {
            rb.AddForce(new Vector2(transform.up.x, transform.up.y) * forwardThrust);
        }
        if (boatController.backwards)
        {
            rb.AddForce(new Vector2(-transform.up.x, -transform.up.y) * reverseThrust);
        }
        if (!boatController.forward && !boatController.backwards)
        {
            rb.velocity -= deceleration * rb.velocity;
        }
        // Apply torque based turning.
        Quaternion lastRotation = transform.rotation;
        if (boatController.left || (boatController.right && boatController.backwards))
        {
            rb.AddTorque(torque * (1 + transform.forward.magnitude));
        }
        if (boatController.right || (boatController.left && boatController.backwards))
        {
            rb.AddTorque(-torque * (1 + transform.forward.magnitude));
        }
        Quaternion currentRotation = transform.rotation;
        Quaternion relativeRotation = currentRotation * Quaternion.Inverse(lastRotation);
        rb.velocity = relativeRotation * rb.velocity;
        // Don't exceed maximum speed.
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }
}

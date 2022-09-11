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

    public AudioSource audioObject;
    public AudioClip paddlingSound;
    bool soundPlaying = false;

    public bool canMove = true;
    private Rigidbody2D rb;

    public BoatController boatController;

    public BoxCollider2D mapBounds;
    private float xMin, xMax, yMin, yMax;

    private void Start()
    {
        boatController = GetComponent<BoatController>();
        if (boatController == null)
        {
            Debug.LogError("Error: Boat Movement does not have attacked Boat Controller");
        }
        rb = GetComponent<Rigidbody2D>();

        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;
    }

    public void Bump(float strength, Vector2 dir, float stunDuration)
    {
        rb.AddForce(dir * strength);
        StartCoroutine(Stun(stunDuration));
    }

    IEnumerator Stun(float stunDuration)
    {
        canMove = false;
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(stunDuration);
        canMove = true;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            
            // Apply forward/reverse thrust and hand-applied deceleration.
            if (boatController.forward)
            {
                rb.AddForce(new Vector2(transform.up.x, transform.up.y) * forwardThrust);
                if(!soundPlaying){
                    audioObject.PlayOneShot(paddlingSound);
                    soundPlaying=true;
                }

                
            }
            if (boatController.backwards)
            {
                rb.AddForce(new Vector2(-transform.up.x, -transform.up.y) * reverseThrust);
                if(!soundPlaying){
                    audioObject.PlayOneShot(paddlingSound);
                    soundPlaying=true;
                }
            }
            if (!boatController.forward && !boatController.backwards)
            {
                rb.velocity -= deceleration * rb.velocity;
                if(!soundPlaying){
                    audioObject.PlayOneShot(paddlingSound);
                    soundPlaying=true;
                }
            }
            // Apply torque based turning.
            Quaternion lastRotation = transform.rotation;
            if (boatController.left || (boatController.right && boatController.backwards))
            {
                rb.AddTorque(torque * (1 + transform.forward.magnitude));
                if(!soundPlaying){
                    audioObject.PlayOneShot(paddlingSound);
                    soundPlaying=true;
                }
            }
            if (boatController.right || (boatController.left && boatController.backwards))
            {
                rb.AddTorque(-torque * (1 + transform.forward.magnitude));
                 if(!soundPlaying){
                    audioObject.PlayOneShot(paddlingSound);
                    soundPlaying=true;
                }
            }
            Quaternion currentRotation = transform.rotation;
            Quaternion relativeRotation = currentRotation * Quaternion.Inverse(lastRotation);
            rb.velocity = relativeRotation * rb.velocity;
            // Don't exceed maximum speed.
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }else{
            audioObject.Stop();
            soundPlaying=false;

        }

        // Don't leave the map if you're the player.
        if (tag == "Player")
        {
            float xBound = Mathf.Clamp(transform.position.x, xMin + 50.0f, xMax - 50.0f);
            float yBound = Mathf.Clamp(transform.position.y, yMin + 50.0f, yMax - 50.0f);
            var smoothPos = Vector3.Lerp(transform.position, new Vector3(xBound, yBound, transform.position.z), 0.5f);
            this.transform.position = smoothPos;
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlpoolEye : MonoBehaviour
{
    public float shrinkRate;
    public float rotationTorque;
    public Transform UniversalWhirlpoolEyeExit;
    private bool running;
    
    private void Awake()
    {
        running = false;
    }

    private void Update() {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<BoatController>() is not null && !running)
        {
            // The player is here!
            StartCoroutine(ShrinkSpinSpitOut(other.gameObject));
        }
    }

    IEnumerator ShrinkSpinSpitOut(GameObject player)
    {
        running = true;
        // Stop their movement.
        var rb = player.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 0.0f);
        // Spin and shrink until invisible.
        var originalScale = player.transform.localScale;
        while (player.transform.localScale.x > 0.1)
        {
            rb.AddTorque(rotationTorque * (1 + transform.forward.magnitude));
            player.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
            yield return null;
        }

        // Move to exit.
        player.transform.position = UniversalWhirlpoolEyeExit.position;

        // Spin and grow until visible at right location.
        while (player.transform.localScale.x < originalScale.x)
        {
            rb.AddTorque(rotationTorque * (1 + transform.forward.magnitude));
            player.transform.localScale += new Vector3(shrinkRate, shrinkRate, shrinkRate);
            yield return null;
        }
        player.transform.localScale = originalScale;
        running = false;
    }
}

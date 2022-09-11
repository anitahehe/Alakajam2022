using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlpoolEye : MonoBehaviour
{
    public float shrinkRate;
    public float rotationTorque;
    public Transform UniversalWhirlpoolEyeExit;
    private bool running;
    public AudioSource AudioObject;
    public AudioClip WhirlpoolSound;
    bool soundPlayed = false;
    
    private void Awake()
    {
        running = false;
    }

    private void Update() {
        UpdateSound();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !running)
        {
            
            // The player is here!
            StartCoroutine(ShrinkSpinSpitOut(other.gameObject));
        }
    }
    private void UpdateSound(){
        if(soundPlayed == false && GetComponentInParent<Whirlpool>().pulling){
            AudioObject.PlayOneShot(WhirlpoolSound);
            soundPlayed = true;
            }
    }

    IEnumerator ShrinkSpinSpitOut(GameObject player)
    {
        running = true;
        // Stop their movement.
        var rb = player.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 0.0f);
        player.GetComponent<BoatMovement>().canMove = false;
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
        
        player.GetComponent<BoatMovement>().canMove = true;
        running = false;
    }
}

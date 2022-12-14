using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlpool : MonoBehaviour
{
    public float pullForceInitial;
    public float pullForceMax;
    public float pullForceIncrement;
    public bool pulling;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // The player is here!
            pulling = true;
            StartCoroutine(PullPlayerCoroutine(other.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag =="Player")
        {
            // The player has escaped!
            pulling = false;
            StopCoroutine(PullPlayerCoroutine(other.gameObject));
        }
    }

    IEnumerator PullPlayerCoroutine(GameObject player)
    {
        var rb = player.GetComponent<Rigidbody2D>();
        float pull = pullForceInitial;
        while (pulling)
        {
            if (pull < pullForceMax)
                pull += pullForceIncrement;
            
            rb.AddForce((transform.position - rb.transform.position).normalized * pull);

            yield return true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougeSceneTouchTrigger : MonoBehaviour
{
    // temp.
    public string convo;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<BoatController>() is not null)
        {
            // Stop the player from moving.
            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // TODO: Start the dialouge scene with the player - these can be their own seperate scenes?
            Debug.Log("START DIALOUGE SCENE: " + convo);
            // TODO: Return to the main scene.
            Debug.Log("END DIALOUGE SCENE: " + convo);
            Destroy(this);
        }
    }
}

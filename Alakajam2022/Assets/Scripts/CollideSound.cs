using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideSound : MonoBehaviour
{
    public AudioSource audioObject;
    public AudioClip crash;
    float delay = 1;
    bool playagain = true;


    // Update is called once per frame
    void Update()
    {
        DecrementTimer();

        
    }
    
    void OnCollisionEnter2D(Collision2D collision){
        if(playagain){
            audioObject.PlayOneShot(crash);
            playagain = false;

        }
        
    }
    void DecrementTimer(){
        if(!playagain){
            delay = (delay-1* Time.deltaTime);
            if(delay <= 0){
                playagain = true;
                delay = 1;
            }
        }

    }
}

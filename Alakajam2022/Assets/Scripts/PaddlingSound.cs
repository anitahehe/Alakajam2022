using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddlingSound : MonoBehaviour
{
    public AudioSource audioObject;
    public float FadeTime;
    bool disableSound;
    // Start is called before the first frame update
    
    public float delay = 1;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("w")){
            audioObject.Play();
            audioObject.volume = 0.25f;
            StopAllCoroutines();
            

        }
        if(Input.GetKeyUp("w")){
            disableSound = true;
          
          StartCoroutine (FadeOut (audioObject, FadeTime));

        }
        if(disableSound){
            DisableSound();
        }
        
    }
     public static IEnumerator FadeOut (AudioSource audioObject, float FadeTime) {
        float startVolume = audioObject.volume;
 
        while (audioObject.volume > 0) {
            audioObject.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioObject.Stop ();
        audioObject.volume = startVolume;
    }
    void DisableSound(){
        
          delay = (delay-1 * Time.deltaTime);
            if(delay <= 0){
                Debug.Log("Stop Sounds");
                //fallback since the couroutine gets stuck in dialog
                audioObject.Stop ();
                delay = 1;
                disableSound = false;
                
            }

    }
}

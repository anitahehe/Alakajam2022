using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddlingSound : MonoBehaviour
{
    public AudioSource audioObject;
    public float FadeTime;
    // Start is called before the first frame update
    
  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("w")){
            audioObject.Play();
            audioObject.volume = 0.5f;
            StopAllCoroutines();

        }
        if(Input.GetKeyUp("w")){
            StartCoroutine (FadeOut (audioObject, FadeTime));
            

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
}

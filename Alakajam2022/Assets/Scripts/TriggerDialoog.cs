using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialoog : MonoBehaviour
{
    public DialogUI dialogUIScript;

    public int storyNumber; 
    public AudioSource audioObject;
    public AudioClip characterMusic;
    bool isTriggered;
    public float FadeTime;


    public bool givesNavigatorItem = false;
    public bool givesShipwrightItem = false;

   void OnTriggerEnter2D(Collider2D collision){
       if(collision.gameObject.tag == "Player"&& !isTriggered){
           Debug.Log("Story is triggered");
           dialogUIScript.InitializeStory(storyNumber);
           audioObject.Play();
           isTriggered = true;
           if (givesNavigatorItem)
           {
                GameManager.Instance().hasNavigatorItem = true;
           }
           if (givesShipwrightItem)
           {
                GameManager.Instance().hasShipwrightItem = true;
           }
       }
   }
   void OnTriggerExit2D(){
    StartCoroutine (FadeOut (audioObject, FadeTime));
    
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

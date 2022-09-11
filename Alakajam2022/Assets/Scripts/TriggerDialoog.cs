using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialoog : MonoBehaviour
{
    public DialogUI dialogUIScript;

    public int storyNumber; 

   void OnTriggerEnter2D(Collider2D collision){
       if(collision.gameObject.tag == "Player"){
           Debug.Log("Story is triggered");
           dialogUIScript.InitializeStory(storyNumber);



       }
   }
}

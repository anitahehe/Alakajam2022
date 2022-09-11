using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialoog : MonoBehaviour
{
    void Start(){
        
    }

   void OnTriggerEnter2D(Collider2D collision){
       if(collision.gameObject.tag == "Player"){
           Debug.Log("Dialog here! BLAH BLAH");
       }
   }
}

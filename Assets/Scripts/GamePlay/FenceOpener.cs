using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceOpener : MonoBehaviour
{
    private void OnTriggerStay(Collider other){

        if(other.tag == "FenceGate")
        {
            Animator anim = other.GetComponentInChildren<Animator>();
            if(Input.GetKeyDown(KeyCode.E)){
                anim.SetTrigger("OpenClose");
            }
        }
    }
}

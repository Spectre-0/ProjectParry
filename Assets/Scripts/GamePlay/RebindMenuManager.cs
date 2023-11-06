using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindMenuManager : MonoBehaviour
{

    public InputActionReference MoveReference ;//, JumpReference , AttackReference , DodgeReference , ParryReference , SprintReference , LookReference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnEnable()
    {
        MoveReference.action.Enable();

    }

    public void OnDisable()
    {
        MoveReference.action.Disable();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

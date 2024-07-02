using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class FellerPlayerInput : MonoBehaviour
{
    // FellerPlayerInput
    // Simply reads the Sniff InputAction from STANKInput, F by default, and calls TakeAWhiff on the Feller.
    // This can be used as-is with the included InputAction or adjust it to use your own Input asset for more control.
    
    Feller feller;    
    STANKInput input;

    // Start is called before the first frame update
    void Start()
    {
        feller = GetComponent<Feller>();
        input = new STANKInput();
        input.gameplay.Sniff.performed += Sniff;
    }

    void Sniff(InputAction.CallbackContext context){
        // Takes a whiff at the player's request
        feller.TakeAWhiff();
    }
}

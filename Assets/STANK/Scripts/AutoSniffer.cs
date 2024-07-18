using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STANK {
    public class AutoSniffer : MonoBehaviour
    {
        // AutoSniffer
        // This script is meant to be attached to the Feller object when you want the Feller to sniff periodically.
        // This is primarily intended for NPCs, but it can be used with a player Feller, as well.

        Feller feller;    
        // sniffInterval is the number of seconds between each sniff
        [SerializeField] float sniffInterval = 1.0f;
        [Range(0,1)]
        [SerializeField] float acuityMultiplier = 1.0f;

        // sniffTimer is the time remaining until the next sniff
        [Range(0.25f, float.PositiveInfinity)]
        float sniffTimer = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            // This needs to be on the same GameObject as the Feller
            feller = GetComponent<Feller>();
            // Set initial sniff timer
            sniffTimer = sniffInterval;
        }

        void ProcessThreshold(STANKResponse response){}

        // Update is called once per frame
        void Update()
        {
            // Countdown to next sniff
            sniffTimer -= Time.deltaTime;
            if(sniffTimer <= 0.0f){
                feller.TakeAWhiff();
                Debug.Log("Autosniff");
                sniffTimer = sniffInterval;
            }
        }
    }
}
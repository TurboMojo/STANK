using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STANK {
    [RequireComponent(typeof(AudioSource))]
    public class STANKResponseAudio : STANKResponseListener, ISTANKResponse
    {
        [HideInInspector]
        public AudioSource aSource;
        
        void Start(){
            aSource = GetComponent<AudioSource>();
            responseEvent.AddListener(ProcessThreshold);
        }

        public void ProcessThreshold(STANKResponse response){
            foreach(STANKResponse a in stankResponse){
                if(a.Stank.name == response.Stank.name){
                    if(aSource.isPlaying == false) aSource.Play();
                }
            }

        }
    }
}
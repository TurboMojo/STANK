using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;


namespace STANK {
    [CreateAssetMenu(menuName = "STANK/STANKResponse")]
    [Serializable]
    public class STANKResponse : ScriptableObject
    {
        // Defines responses to a STANKTolerance
        // Can trigger an AnimationClip, which is built into the AnimatorController by STANKYLeg
        // The responseVFX ParticleSystem is intended for things like tears, vomit, etc.
        // STANKResponseAudio
        [Header("Required Fields")]
        public Stank stank;
        // STANKTolerance object that defines the threshold for this response.
        public float pungencyThreshold = 0.5f;
        [Tooltip("Minimum amount of time required between responses, prevents infinite puke loops")]
        [Min(0.1f)]
        public float responseDelay = 10.0f;      
        public float delayTimer = 0.0f;
        
        [Header("Optional Fields")]
        // AnimationClip will be triggered when toleranceThreshold is reached.  This does not need to be added to your AnimatorController.  STANKYLeg will do it for you.
        public AnimationClip AnimationClip;    
        // ResponseAudio should contain all the AudioClips that should be played when the STANKTolerance is reached.
        public AudioClip[] ResponseAudio;
        // Ragdoll support is intended for actions such as fainting or death.  If this is set, isKinematic on all Rigidbodies in the RagdollRoot will be disabled.
        public Rigidbody RagdollRoot;

        private readonly List<STANKResponseListener> listeners = new List<STANKResponseListener>();

        void Awake(){
            
        }

        public void Respond(){
            delayTimer = responseDelay;
            for(int i = 0; i < listeners.Count; i++){
                listeners[i].OnEventRaised(this);
            }
        }


        public void RegisterListener(STANKResponseListener listener){
            delayTimer = 0;
            if(!listeners.Contains(listener)) {
                //Debug.Log("Registering listener: "+listener.name);
                listeners.Add(listener);
            } else {
                Debug.Log("Listener already registered");
            }
        }

        public void UnregisterListener(STANKResponseListener listener){
            
            if(listeners.Contains(listener)) listeners.Remove(listener);
            else Debug.Log("Listener already unregistered");            
        }

        void Update(){
            if(delayTimer > 0) delayTimer -= Time.deltaTime;
        }
    }
}
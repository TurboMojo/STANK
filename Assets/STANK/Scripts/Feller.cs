using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

namespace STANK {
    public class Feller : MonoBehaviour
    {
        // A Feller is any object that detects smells
        // This can be your player, NPCs, a fancy computer that smells things, whatever you want.
        // Every Feller must have at least one STANKTolerance defined per Stank to which it should react.
        // This should be attached to a child of your Feller, placed wherever you feel appropriate for your purposes.
        // It may be useful in some cases to make the nose bone of your armature the Feller's parent, for instance.
        // This component requires the use of another component:
        // - FellerPlayerInput
        //       Accepts player input to take a whiff at smells.  This is intended for experiences where smelling the environment is an action
        //       to be taken by the player.
        // - AutoSniffer (see below)
        //       Automatically sniffs periodically.  This is intended for NPCs and players who should passively be aware of smells in the environment.

        
        [Header("Required Fields")]    
        [Tooltip("STANKResponses for all the Stanks to which this Feller should respond.")]
        public List<STANKResponse> responses;
        [Tooltip("How well this feller smells")]
        public float acuity = 1.0f;
        /* [Tooltip("Delay in seconds between reactions.  This MUST be > 0, or no reactions will ever successfully occur.")]
        public float reactionDelay = 10.0f;
        float delayTimer = 0.0f; */
        [Header("Optional Fields")]
        // STANKEye and STANKYLeg are optional, but if they are not present, the Feller will not react to smells unless supported by custom components.  Must be attached to the Feller gameobject.
        STANKEye stankeye;
        STANKYLeg stankyleg;
        
        // collisionEvents is used to detect particle collisions.  This is currently useable, but undocumented. It may be re-implemented in the future
        // to allow for more complex simulations of stank propagation through an environment, but is not a current priority.    
        List<ParticleCollisionEvent> collisionEvents;
        
        // detectedOdors is a list of all the STANKs that this Feller has detected.    
        [HideInInspector] public List<Stank> detectedSTANKs;
        
        // detectedSmellers is a list of all the Smellers that this Feller has detected.    
        [HideInInspector] public List<Smeller> detectedSmellers;
        
        // undetectedOdors is a list of all the Smellers that this Feller has not detected.    
        [HideInInspector] public List<Smeller> undetectedSmellers;    
        

        
        float blockerPermeability = 0.0f;

        public void Initialize(){
            stankeye = GetComponent<STANKEye>();     
            stankyleg = GetComponent<STANKYLeg>();   
            detectedSTANKs = new List<Stank>();
            detectedSmellers = new List<Smeller>();
            undetectedSmellers = Olfactory.Instance.smellers;        
            if(stankeye != null) stankeye.Initialize();
            if(stankyleg != null) stankyleg.Initialize(this);
        }

        // DecaySTANKs in Update() will decay the stank levels of all detected stanks
        void Update()
        {
            // Find any Smellers that this Feller can smell and determine how intensely we smell those smells.
            CalculatePungency();
            // Check our currently smelled Stanks and see if we smell any of them enough to trigger a STANKResponse.
            CheckPerception();
        }

        public float GetHighestToleranceValue(Stank odor)
        {
            // Get the highest tolerance value for the given stank for this Feller
            float highestToleranceValue = float.MinValue; 

            // Simple sorting operation to find the highest tolerance value of all our responses
            if (odor != null && responses != null)
            {
                foreach (STANKResponse response in responses.ToList())
                {
                    if(response == null) {
                        // I have absolutely no idea why this is necessary, but it is.
                        responses.Remove(response);
                        continue;
                    }
                    if (response.pungencyThreshold > highestToleranceValue)
                    {
                        highestToleranceValue = response.pungencyThreshold;
                    }
                }
            }
            return highestToleranceValue;
        }

        public float GetStankLevel(Stank stank)
        {
            // Return the current stank level for this Feller for the given stank
            foreach (Stank s in detectedSTANKs)
            {
                if (s.name == stank.name)
                {
                    return s.Pungency;
                }
            }
            return 0;
        }

        public void TakeAWhiff()
        {        
            // Detects nearby smellers and calculates the stank level of each one for this Feller.
            // Called from user input.  Requires either a FellerPlayerInput or AutoSniffer to be attached to the Feller.
            
            undetectedSmellers.Clear();        
            undetectedSmellers = FindObjectsOfType<Smeller>().ToList();
            // Raycast to see if there is a STANKBlocker between this Feller and the Smeller.  If there is, we don't count that as a smell.
            RaycastHit hit;
            foreach (Smeller s in undetectedSmellers.ToList()){
                if(Vector3.Distance(transform.position, s.transform.position) < s.radius ) {
                    if(Physics.Raycast(transform.position, s.transform.position - transform.position, out hit, s.radius)){
                        // Blocker in the way.  Stank occluded.
                        if(hit.collider.GetComponent<STANKluder>() != null) blockerPermeability = hit.collider.GetComponent<STANKluder>().permeability;
                    }
                    // No blocker in the way.  Feller smells this Smeller.
                    detectedSmellers.Add(s);
                    undetectedSmellers.Remove(s);                
                    
                } else if(detectedSmellers.Contains(s)) {
                    // We've left the Smeller's range.  Feller no longer smells this Smeller.
                    detectedSmellers.Remove(s);
                    detectedSTANKs.Remove(s.stank);
                    undetectedSmellers.Add(s);
                    // Set the reaction delay back to 0, in anticipation of a new response.
                    //delayTimer = 0;
                }
            }

            // We populate our public list of detected STANKs for easy access by dependent scripts, such as STANKEye and STANKYLeg.
            foreach(Smeller smeller in detectedSmellers){
                if(!detectedSTANKs.Contains(smeller.stank)){
                    Stank newStank = new Stank();
                    newStank.name = smeller.stank.name;
                    newStank.Pungency = smeller.stank.Pungency;
                    newStank.response = smeller.stank.response;
                    if(smeller.stank.Description != "") newStank.Description = smeller.stank.Description;
                    if(smeller.stank.Icon != null) newStank.Icon = smeller.stank.Icon;
                    if(smeller.stank.HUDMaterial != null) newStank.HUDMaterial = smeller.stank.HUDMaterial;
                    if(smeller.stank.HUDIcon != null) newStank.HUDIcon = smeller.stank.HUDIcon;
                    if(smeller.stank.Smeller != null) newStank.Smeller = smeller.stank.Smeller;
                    detectedSTANKs.Add(newStank);
                }
            }

            // Make sure STANKEye is up to date
            if(stankeye) stankeye.RefreshHUD();
        }

        void CheckPerception()
        {
            // Check this Feller's perception of all detected STANKs.  If the pungency of any is above the tolerance threshold, we trigger a STANKResponse and broadcast it to all components on this gameObject and all its children.
            foreach(Stank o in detectedSTANKs)
            {
                if (o.Pungency > o.response.pungencyThreshold && o.response.delayTimer <= 0)
                {                       
                    o.response.Respond();
                    o.response.delayTimer = o.response.responseDelay;                    
                }
            }
        }

        public float GetPungency(Stank stank){
            // Returns the pungency of the given Stank as perceived by this Feller.
            foreach(Stank s in detectedSTANKs){
                if(s.name == stank.name){
                    return s.Pungency;
                }
            }
            return 0f;
        }

        public Stank DetectedStank(){
            // Used for Debugging.  Likely obsolete.
            return detectedSTANKs[0];
        }

        void CalculatePungency(){
            foreach (Stank stank in detectedSTANKs.ToList())
            {
                // Calculate our distance as a percentage of the Smeller's radius.
                float radiusPercentage = Vector3.Distance(transform.position, stank.Smeller.transform.position) / stank.Smeller.radius;
                
                // Pungency is based on the Smeller's pungency curve.  A smell is strongest at its source.  The pungency curve dictates the pungency dropoff along the radius of the Smeller.
                if(stank.Pungency * stank.Smeller.pungencyCurve.Evaluate(radiusPercentage) <= GetHighestToleranceValue(stank))
                {
                    //Debug.Log("Calculate pungency A: "+stank.Pungency);
                    // If the Stank's pungency is below the highest tolerance value, we apply the pungency dropoff.
                    stank.Pungency = stank.Smeller.pungencyCurve.Evaluate(radiusPercentage) *acuity;
                    //stank.Pungency = stank.Smeller.pungencyCurve.Evaluate(radiusPercentage);
                }
                else{
                    //Debug.Log("Calculate pungency B: "+stank.Pungency);
                    // If the Stank's pungency is above the highest tolerance value, we default to the highest tolerance value.
                    stank.Pungency = GetHighestToleranceValue(stank);
                }
                
                
                if(stank.Pungency <= 0 ) 
                {
                    // If the Stank's pungency is below 0, we remove it from our list, as it is no longer detected.
                    detectedSTANKs.Remove(stank);

                    // We also cancel our reaction delay timer
                    //delayTimer = 0f;
                }
            }
        }            
    } 
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Assertions.Must;


namespace STANK {
    public class STANKYLeg : STANKResponseListener, ISTANKResponse
    {
        // STANKYLeg is the class that manages animations triggered by STANKResponses.
        // An AnimatorControllerLayer is built at runtime and added to the AnimatorController.
        // This layer is prioritized on-demand to 
        Animator anim;
        STANKEye sTANKEye;
        AnimatorControllerLayer stankLayer;
        Feller feller;

        [Tooltip("Optional avatar mask to define which bones will respond to response animations")]
        [SerializeField] AvatarMask avatarMask;
        [Tooltip("Optional Rigidbody to ragdoll when response animations are triggered if reaction triggers fainting.")]
        [SerializeField] Rigidbody ragdollRoot;
        //float currentreactionDelayTimer;
        float layerTimer = 0f;
        int layerIndex = 0;

        // Start is called before the first frame update
        public void Initialize(Feller f)
        {
            if(anim == null) anim = transform.parent.GetComponent<Animator>();
            sTANKEye = transform.parent.GetComponentInChildren<STANKEye>();
            feller = f;

            // STANKYLeg should always be using the responses defined in the Feller, rather than responses defined in the stankResponse list from STANKResponseListener.
            if(stankResponse.Count > 0) stankResponse = feller.responses;
            BuildSTANKLayer();
        }

        void BuildSTANKLayer()
        {   
            // Create a layer in the AnimatorController for STANKResponses
            if (avatarMask != null) stankLayer = CreateSTANKLayer(anim, avatarMask);           
            else stankLayer = CreateSTANKLayer(anim);
        }

        public AnimatorControllerLayer CreateSTANKLayer(Animator animator, AvatarMask avatarMask)
        {
            // Get the AnimatorController from the Animator
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;

            // Check if the STANKYLeg layer already exists.  If so, delete it and build a new one.
            RemoveSTANKLayer(animatorController);

            // Create a layer in the AnimatorController and name it
            AnimatorControllerLayer layer = new AnimatorControllerLayer();
            layer.name = "StankyLeg";
            layer.stateMachine = new AnimatorStateMachine();
            layer.stateMachine.name = layer.name;
            // Set the AvatarMask for the layer
            layer.avatarMask = avatarMask;
            // Add an Animator state for each STANKResponse
            foreach (STANKResponse response in feller.responses)
            {
                AddStateToLayer(stankLayer, response);
            }
            animatorController.AddLayer(layer);
            layerIndex = anim.GetLayerIndex(layer.name);
            AssetDatabase.AddObjectToAsset(layer.stateMachine, animatorController);
            // Save the changes to the AnimatorController
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return layer;
        }

        void RemoveSTANKLayer(AnimatorController animatorController){
            int layerIndex = 1;
            foreach(AnimatorControllerLayer i in animatorController.layers){
                if(i.name == "StankyLeg"){
                    AssetDatabase.RemoveObjectFromAsset(i.stateMachine);
                    animatorController.RemoveLayer(layerIndex);
                    layerIndex++;
                }
            }
            RemoveSTANKLayerParameters(animatorController);
        }

        void RemoveSTANKLayerParameters(AnimatorController animatorController){
            foreach(AnimatorControllerParameter i in animatorController.parameters){
                foreach(Stank s in Olfactory.Instance.stanks){
                    if(i.name == s.name){
                        animatorController.RemoveParameter(i);
                    }
                }
            }
        }

        public AnimatorControllerLayer CreateSTANKLayer(Animator animator)
        {
            // Check if the AnimatorController already exists
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;

            if (animatorController == null)
            {
                return null;
            }

            // Check if the AnimatorController already exists.  If so, delete it and build a new one.
            RemoveSTANKLayer(animatorController);

            // Create a layer in the AnimatorController
            AnimatorControllerLayer layer = new AnimatorControllerLayer();
            layer.name = "StankyLeg";
            layer.stateMachine = new AnimatorStateMachine();
            layer.stateMachine.name = layer.name;

            // Add an Animator state for each STANKResponse
            foreach (STANKResponse response in feller.responses)
            {
                if(response == null) continue;
                AddStateToLayer(layer, response);
            }
            animatorController.AddLayer(layer);
            layerIndex = anim.GetLayerIndex(layer.name);
            AssetDatabase.AddObjectToAsset(layer.stateMachine, animatorController);
            // Save the changes to the AnimatorController
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return layer;
        }

        public List<AnimatorState> GetAnimatorStateInfo(Animator animator)
        {
            // Get all the AnimatorStates in the AnimatorController
            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            AnimatorControllerLayer[] acLayers = ac.layers;
            List<AnimatorState> allStates = new List<AnimatorState>();
            foreach (AnimatorControllerLayer i in acLayers)
            {
                ChildAnimatorState[] animStates = i.stateMachine.states;
                foreach (ChildAnimatorState j in animStates)
                {
                    allStates.Add(j.state);
                }
            }
            return allStates;
        }

        public AnimatorState GetStateByName(AnimatorControllerLayer layer, string name)
        {
            List<AnimatorState> states = GetAnimatorStateInfo(anim);
            foreach(AnimatorState state in states)
            {
                if (state.name == name) return state;
            }
            return new AnimatorState(); ;
        }

        // Method to add a state to an existing AnimatorController layer with Trigger transitions
        public void AddStateToLayer(AnimatorControllerLayer layer, STANKResponse response)
        {
            // Add a state for this response animation to STANKLayer
            if(response.AnimationClip == null || anim == null) return;        
                
            AnimatorController ac = (AnimatorController) anim.runtimeAnimatorController;
            
            // Create a state and entry transition in the layer
            AnimatorState state = layer.stateMachine.AddState(response.AnimationClip.name);
            layer.stateMachine.AddEntryTransition(state);

            // Create a Trigger parameter in the AnimatorController named for the Stank defined in response
            AnimatorControllerParameter p = new AnimatorControllerParameter();
            p.type = AnimatorControllerParameterType.Trigger;
            p.name = response.stank.name;
            ac.AddParameter(p);

            // Assign the AnimationClip to the state
            state.motion = response.AnimationClip;

            // Create a Trigger transition from the Entry node to the new state
            AnimatorTransitionBase entryToStateTransition = layer.stateMachine.AddAnyStateTransition(state);
            entryToStateTransition.AddCondition(AnimatorConditionMode.If, 0, response.stank.name);
            entryToStateTransition.destinationState = state;

            // Create a Trigger transition from the new state to the Exit node
            AnimatorTransitionBase stateToExitTransition = state.AddExitTransition();
            stateToExitTransition.AddCondition(AnimatorConditionMode.If, 0, response.stank.name);
            stateToExitTransition.destinationState = GetStateByName(layer, "Exit");

            // Save the changes to the AnimatorController
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void PlayAnimation(STANKResponse response){
            // If an animation is defined for the response, set the weight of the StankLayer to 1 and play the animation.  layerTimer assures that we return to the main Animator FSM after our Response animation has finished
            if (response.AnimationClip != null)
            {
                anim.SetLayerWeight(anim.GetLayerIndex("StankyLeg"), 1f);
                anim.SetTrigger(response.stank.name);
                layerTimer = response.AnimationClip.length;
            }
        }

        public void ProcessThreshold(STANKResponse response){
            // Feller tells us we've reached the threshold for a response, time to trigger it, as long as the AnimationClip is defined.

            if(response.AnimationClip == null) return;
            
            // If a RagdollRoot is defined, faint the Feller and return.
            if(ragdollRoot != null){
                Faint(response);
                return;
            }

            // If no RagdollRoot is defined, set the weight of the StankLayer to 1 and play the animation
            anim.SetLayerWeight(anim.GetLayerIndex("StankyLeg"), 1f);
            PlayAnimation(response);
            // LayerTimer lets us return to the main Animator FSM after our Response animation has finished
            layerTimer = response.AnimationClip.length;
        }

        void Faint(STANKResponse response){
            // We ragdoll the RagdollRoot and all its Rigidbody children
            foreach(Rigidbody r in ragdollRoot.gameObject.GetComponentsInChildren<Rigidbody>()){
                r.isKinematic = false;
            }
        }

        void Update()
        {
            // Return to the main Animator FSM after our Response animation has finished
            if (layerTimer > 0)
            {
                layerTimer -= Time.deltaTime;
                if (layerTimer <= 0)
                {
                    anim.SetLayerWeight(anim.GetLayerIndex("StankyLeg"), 0f);
                    layerTimer = 0;

                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace STANK {
    public class Smeller : MonoBehaviour
    {
        // Smellers are the active force providing STANKs to your scene.
        // This component should be attached to anything that emits a STANK.
        // Any feller within radius will become aware of the STANK and react according to the Feller's STANKTolerances.
        // The attenuationCurve AnimationCurve describes how the STANK level changes with distance from the emitter.
        // It should be a curve with the first keyframe at 0 and the last keyframe at 1.
        // The first keyframe describes the farthest outer edge of the Smeller's influence.
        // The last keyframe describes the center of the Smeller's influence.

        // Radius around the GameObject where the STANK will be detected by Fellers.
        public float radius = 10f;
        // The odor emitted by this Smeller
        public Stank stank;
        // pungencyCurve describes the attenuation of the STANK level based on distance from the Smeller.
        public AnimationCurve pungencyCurve;
        public Stank Stank { get => stank;}
        // The rate at which the Smeller's radius expands (meters per second)
        [SerializeField] float expansionRate = 1.0f;

        [Header("Optional Fields")]
        [HideInInspector] public Image hudImage;
        // Whether stink lines should be drawn above the emitter
        public bool ShowStankLines;
        // Emitter for stink lines
        public ParticleSystem[] StankLinesEmitters;    
        
        private void OnDrawGizmos(){
            if(Stank == null) return;
            // Visualize a sphere to describe the Smeller's radius in the inspector.
            Gizmos.color = stank.GizmoColor;
            if(stank != null) {
                Gizmos.DrawSphere(transform.position, radius);
                Gizmos.DrawIcon(transform.position, "STANKGizmo.png", true, Color.white);
                if(StankLinesEmitters != null){
                    foreach(ParticleSystem p in StankLinesEmitters){
                        ParticleSystem.ShapeModule StankLineShape = p.shape;
                        StankLineShape.radius = radius;    
                    }                
                }
            }
        }

        void Start(){
            stank.Smeller = this;
        }

        void Update(){
            radius += expansionRate * Time.deltaTime;
        }
    }
}
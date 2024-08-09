using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace STANK {
    [CreateAssetMenu(menuName = "STANK/Stank")]
    [Serializable]
    public class Stank : ScriptableObject
    {
        // A Stank defines the smells that Fellers can smell.
        // A Smeller requires a Stank to be detected by a Feller.
        // Each feller will need at least one StankTolerance for each Stank.
        // StankTolerances are used to determine thresholds for how a Stank will effect a Feller.

        [Header("Required Fields")]
        // Name of the Stank.  Can be used to display information to the player
        public string Name = "";
        // Pungency is the ratio between a Feller's tolerance for this Stank and the CurrentStankLevel.  1.0 is calculated as the largest defined tolerance threshold for the Feller.
        [HideInInspector] public float Pungency = 0f;
        public Color GizmoColor = Color.green;

        [Header("Optional Fields")]
        // Descriptive name of the Stank.    
        public string Description = "";
        // Icon to display in HUD
        public Texture2D Icon;
        public Material HUDMaterial;

        [HideInInspector] public Image HUDIcon;
        // Particle system that emits the Stank
        [HideInInspector] public Smeller Smeller;
    }
}
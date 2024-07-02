using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "STANK/STANKEffect")]
[Serializable]
public class STANKEffect : ScriptableObject
{
    public GameObject effectParticles;
    public float maxParticlesPerSecond = 100;
    public float startSize = 0f;
        
    // Start is called before the first frame update
    void Start()
    {
        maxParticlesPerSecond = effectParticles.GetComponent<ParticleSystem>().emission.rateOverTime.constantMax;
        startSize = effectParticles.GetComponent<ParticleSystem>().main.startSize.constant;
    }
}

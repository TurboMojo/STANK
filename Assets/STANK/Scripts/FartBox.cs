using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// This component returns the particle system to the pool when the OnFartStopped event is received.
[RequireComponent(typeof(Fart))]
public class ReturnToFartPool : MonoBehaviour
{
    public Fart fart;
    public IObjectPool<Fart> pool;

    void Start()
    {
        fart = GetComponent<Fart>();
    }

    void OnDestroy(){
        // Return to the pool
        pool.Release(fart);
    }
}

public class FartBox : MonoBehaviour
{

    // Fartbox is responsible for spawning FARTS (Floating/Aerosol/Roaming/Temporary Smellers)
    
    public GameObject fartPrefab;
    public int amountToPool = 100;
    public float lingerDuration = 0.0f;
    float lingerTimer = 0.0f;
    public float velocity = 0.0f;
    public float radius = 0.0f;
    public AnimationCurve lingerCurve;

    public void Fart(){
        GameObject fart = GameObject.Instantiate(fartPrefab, transform.position, Quaternion.identity);
    }
}
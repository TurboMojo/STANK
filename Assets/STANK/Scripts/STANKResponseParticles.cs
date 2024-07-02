using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class STANKResponseParticles : STANKResponseListener, ISTANKResponse
{
    ParticleSystem ps;
    
    
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        responseEvent.AddListener(ProcessThreshold);
    }


    public void ProcessThreshold(STANKResponse response){
        Debug.Log($"StankResponse: {stankResponse.stank.name} Response: {response.stank.name}");
        if(stankResponse.name == response.name){
            ps.Play(true);            
        }
    }
}

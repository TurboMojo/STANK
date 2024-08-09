using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STANK {
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
              
            foreach(STANKResponse a in stankResponse){
                if(a.name == response.name){
                    //Debug.Log("Playing Response: "+response.name+" on "+gameObject.name);
                    //Debug.Log("a.name: "+a.name);
                    ps.Play(true);            
                }
            }            
        }
    }
}
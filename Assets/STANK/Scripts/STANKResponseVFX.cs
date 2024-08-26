using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STANK {
    public class STANKResponseVFX : MonoBehaviour
    {
        ParticleSystem ps;
        public Stank stank;
        float reactionTimer = 0f;
        
        // Start is called before the first frame update
        void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        void Update(){
            if(reactionTimer > 0) reactionTimer -= Time.deltaTime;
        }

        public void ProcessThreshold(STANKResponse response){
            
            if(response.Stank.name == stank.name && reactionTimer <= 0){
                ps.Play();
                reactionTimer = response.ResponseDelay;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace STANK {
    [System.Serializable] public class STANKResponseEvent : UnityEvent<STANKResponse> {}

    public class STANKResponseListener : MonoBehaviour
    {
        public List<STANKResponse> stankResponse;
        [HideInInspector]
        public STANKResponseEvent responseEvent = new STANKResponseEvent();

        private void OnEnable(){
            foreach(STANKResponse a in stankResponse){
                a.RegisterListener(this);
            }
            
        }

        private void OnDisable(){
            foreach(STANKResponse a in stankResponse){
                a.UnregisterListener(this);
            }
        }

        public void OnEventRaised(STANKResponse response){
            responseEvent.Invoke(response);
        }
    }
}
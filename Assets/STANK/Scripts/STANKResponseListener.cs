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
                // Debug.Log("Registering listener: "+a.name+ "on "+gameObject.name);
                a.RegisterListener(this);
            }
            
        }

        private void OnDisable(){
            foreach(STANKResponse a in stankResponse){
                //Debug.Log("Unregistering listener: "+a.name+ " on "+gameObject.name);
                a.UnregisterListener(this);
            }
        }

        public void OnEventRaised(STANKResponse response){
            //Debug.Log("Event raised on: "+response.name);
            responseEvent.Invoke(response);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace STANK {
    [System.Serializable] public class STANKResponseEvent : UnityEvent<STANKResponse> {}

    public class STANKResponseListener : MonoBehaviour
    {
        public STANKResponse stankResponse;
        [HideInInspector]
        public STANKResponseEvent responseEvent = new STANKResponseEvent();

        private void OnEnable(){
            Debug.Log($"Registering listener {gameObject.name}");
            stankResponse.RegisterListener(this);
        }

        private void OnDisable(){
            stankResponse.UnregisterListener(this);
        }

        public void OnEventRaised(STANKResponse response){
            responseEvent.Invoke(response);
        }
    }
}
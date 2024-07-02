using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class STANKResponseEvent : UnityEvent<STANKResponse> {}

public class STANKResponseListener : MonoBehaviour
{
    public STANKResponse stankResponse;
    [HideInInspector]
    public STANKResponseEvent responseEvent = new STANKResponseEvent();

    private void OnEnable(){
        stankResponse.RegisterListener(this);
    }

    private void OnDisable(){
        stankResponse.UnregisterListener(this);
    }

    public void OnEventRaised(STANKResponse response){
        responseEvent.Invoke(response);
    }
}

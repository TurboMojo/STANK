using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class STANKEye : MonoBehaviour
{

    // STANKEye is the class that manages all visual effects related to STANKs.    
    float smellDetectionTickRate = 1.0f;
    
    // Parent gameobject positioned in front of the camera so that it's properly visible.
    [SerializeField] GameObject fullscreenParticleParent;
    // Parent gameobject for positioning particles that will interact with the world, such as vomit or sneeze particles.
    [SerializeField] GameObject worldspaceParticleParent;
    STANKEyecon[] sTANKEyecons;
    List<GameObject> sTANKEyeAlphaGOs;

    // The feller that owns this STANKEye
    Feller feller;


    public void Initialize(){
        feller = transform.GetComponent<Feller>();
        sTANKEyecons = GetComponentsInChildren<STANKEyecon>();
        sTANKEyeAlphaGOs = new List<GameObject>();
        foreach(STANKEyecon a in sTANKEyecons){
            a.gameObject.SetActive(false);
            sTANKEyeAlphaGOs.Add(a.gameObject);
        }
    }

    void Update(){
        UpdateStinkLines();
        WakeUpIcons();
    }

    void WakeUpIcons(){
        foreach(Stank s in feller.detectedSTANKs.ToList()){            
            foreach(STANKEyecon a in sTANKEyecons){
                if(s.name == a.stank.name){                    
                    if(a.gameObject.activeSelf == false){
                        if(s.Pungency > 0){
                            a.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    void UpdateStinkLines()
    {
        // WIP
    }

    public void OpenEye()
    {
        // Called by Olfactory to start STANKEye once required components have been initialized
        StartCoroutine(UpdateHUD());
    }

    public void RefreshHUD(){
        // Public method to trigger a HUD update for player-initiated sniffing.
        foreach (Feller feller in Olfactory.Instance.fellers)
        {
            if (feller.detectedSTANKs.Count() > 0)
            {
                foreach(STANKEyecon a in sTANKEyecons){
                    if(feller.name == a.name && a.gameObject.activeSelf == false){
                        a.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    IEnumerator UpdateHUD()
    {
        // Update all HUD icons for all Fellers in the scene.
        RefreshHUD();
        yield return new WaitForSeconds(smellDetectionTickRate);
        StartCoroutine(UpdateHUD());
    }
}

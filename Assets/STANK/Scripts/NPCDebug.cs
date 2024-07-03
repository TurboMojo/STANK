using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace STANK {
    public class NPCDebug : MonoBehaviour
    {
        Feller feller;
        Canvas debugCanvas;
        Text detectedStankText;
        Text detectedPungencyText;

        // Start is called before the first frame update
        void Start()
        {        
            debugCanvas = GetComponent<Canvas>();
            debugCanvas.worldCamera = Camera.main;
            feller = transform.parent.GetComponent<Feller>();
            detectedPungencyText = debugCanvas.transform.Find("PungencyText").GetComponent<Text>();
            detectedStankText = debugCanvas.transform.Find("StankText").GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if(feller.detectedSTANKs.Count > 0){
                detectedStankText.text = feller.DetectedStank().ToString();
                detectedPungencyText.text = feller.GetPungency(feller.DetectedStank()).ToString();
            } else {
                detectedStankText.text = "None";
                detectedPungencyText.text = 0.ToString();
            }
            
            
        }
    }
}
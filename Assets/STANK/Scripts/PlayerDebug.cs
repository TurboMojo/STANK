using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace STANK {
    public class PlayerDebug : MonoBehaviour
    {
        Feller feller;
        Text stankText;
        Text pungencyText;

        // Start is called before the first frame update
        void Start()
        {
            feller = GetComponentInParent<Feller>();
            stankText = GameObject.Find("StankText").GetComponent<Text>();
            pungencyText = GameObject.Find("PungencyText").GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if(feller.detectedSTANKs == null || feller.detectedSTANKs.Count == 0) return;
            stankText.text = feller.DetectedStank().name;
            pungencyText.text = feller.DetectedStank().Pungency.ToString();
        }
    }
}
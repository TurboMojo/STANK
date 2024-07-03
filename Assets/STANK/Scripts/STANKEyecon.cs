using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace STANK {
    public class STANKEyecon : MonoBehaviour
    {
        Feller feller;
        Image icon;
        Material alphaMat;
        public Stank stank;
        bool foundStank = false;
        [SerializeField] bool variableAlpha = false;

        // Start is called before the first frame update
        void Start()
        {
            feller = GetComponentInParent<Feller>();
            if(GetComponent<Image>() != null){
                alphaMat = GetComponent<Image>().material;
            }
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < feller.detectedSTANKs.Count; i++){
                if(feller.detectedSTANKs[i].name == stank.name) foundStank = true;
            }
            if(foundStank == false) {
                gameObject.SetActive(false);            
            }
            if(variableAlpha == false) return;
            Color alphaColor = alphaMat.color;
            alphaColor.a = feller.GetPungency(stank);
            alphaMat.color = alphaColor;
            foundStank = false;
        }
    }
}
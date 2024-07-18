using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace STANK {
    public class Olfactory : MonoBehaviour
    {
        public static Olfactory Instance { get; private set; }

        public List<Feller> fellers;
        public List<Smeller> smellers;    
        public List<Stank> stanks;
        STANKEar[] stankEars;
        [SerializeField] AudioMixer mixer;
        [SerializeField] AudioMixerGroup mixerGroup;

        [SerializeField] Transform hudParent;    

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            smellers = FindObjectsOfType<Smeller>().ToList();        
            fellers = FindObjectsOfType<Feller>().ToList();
            stankEars = FindObjectsOfType<STANKEar>();

            foreach(STANKEar ear in stankEars){
                if(mixer != null){
                    ear.SetAudioMixer(mixer);
                }                
            }
            
            foreach(Smeller smeller in smellers)
            {
                if(smeller.stank.Icon != null && smeller.stank.HUDIcon != null) smeller.stank.HUDIcon = CreateHUDIcon(smeller.stank);
                if(!stanks.Contains(smeller.stank)) stanks.Add(smeller.stank);
            }
            if (fellers.Count > 0)
            {
                foreach (Feller feller in fellers.ToList())
                {
                    AddFeller(feller);
                    feller.Initialize();
                }
            }
        }

        Image CreateHUDIcon(Stank odor)
        {
            Debug.Log("CreateHUDIcon");
            GameObject imgObject = new GameObject(odor.name+"_HUDIcon");
            imgObject.transform.SetParent(hudParent);
            RectTransform trans = imgObject.AddComponent<RectTransform>();
            trans.localScale = Vector3.one;
            trans.anchoredPosition = Vector2.zero; // setting position, will be on center
            trans.sizeDelta = new Vector2(odor.Icon.width, odor.Icon.height); // custom size

            Image image = imgObject.AddComponent<Image>();
            image.material = odor.HUDMaterial;
            Texture2D tex = odor.Icon;
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            imgObject.transform.SetParent(hudParent);
            imgObject.SetActive(false);
            return image;
        }

        public void AddFeller(Feller f)
        {
            fellers.Add(f);
        }
    }
}
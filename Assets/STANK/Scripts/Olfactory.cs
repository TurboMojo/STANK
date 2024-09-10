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
        [SerializeField] AudioMixer mixer;
        [SerializeField] AudioMixerGroup mixerGroup;
        [SerializeField] Transform hudParent;    

        // Start is called before the first frame update
        void Start()
        {
            // Variable initialization
            Instance = this;
            smellers = FindObjectsOfType<Smeller>().ToList();        
            fellers = FindObjectsOfType<Feller>().ToList();
            
            // Make sure all HUDIcons exist when appropriate
            foreach(Smeller smeller in smellers)
            {
                if(smeller.stank.Icon != null && smeller.stank.HUDIcon != null) smeller.stank.HUDIcon = CreateHUDIcon(smeller.stank);
                if(!stanks.Contains(smeller.stank)) stanks.Add(smeller.stank);
            }
            
            // Add all Fellers to an accessible list and call Initialize() to set up the Fellers.
            foreach (Feller feller in fellers.ToList())
            {
                fellers.Add(feller);
                feller.Initialize();
            }            
        }

        Image CreateHUDIcon(Stank odor)
        {
            // Create a HUD Icon for each odor and set its position and size
            GameObject imgObject = new GameObject(odor.name+"_HUDIcon");
            RectTransform trans = imgObject.AddComponent<RectTransform>();
            trans.localScale = Vector3.one;
            trans.anchoredPosition = Vector2.zero; // setting position, will be on center
            trans.sizeDelta = new Vector2(odor.Icon.width, odor.Icon.height); // custom size

            // Create an image component and set its material and sprite
            Image image = imgObject.AddComponent<Image>();
            image.material = odor.HUDMaterial;
            Texture2D tex = odor.Icon;
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            // Parent the image to the HUD parent and make it invisible
            imgObject.transform.SetParent(hudParent);
            imgObject.SetActive(false);
            return image;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STANKEye_HUDIcon : STANKEye
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHUDIcon(Stank stank, Feller feller)
    {
        // Sets opacity for any active HUD icons to an Odor's pungency on a Feller
        
        foreach(Feller f in Olfactory.Instance.fellers)
        {
            if (f.GetStankLevel(stank) == 0)
            {
                if(stank.Icon != null)   stank.HUDIcon.gameObject.SetActive(false);
                Debug.Log("Stank level is 0");
                return;
            }
            else
            {
                if (stank.Icon != null && stank.HUDIcon != null)
                {
                    stank.HUDIcon.gameObject.SetActive(true);
                } else
                {
                    Debug.Log("Can't find icon");
                }
            }


            if (stank.HUDMaterial != null)
            {
                float stankLevel = f.GetStankLevel(stank);
                // Can be used to help define custom materials per STANKTolerance
                // Currently considered unnecessary and confusing, but will keep commented in case a need arises.
                //odor.HUDMaterial = Olfactory.Instance.GetStankTolerance(f, odor, stankLevel).Material;
                Material tempMat = stank.HUDMaterial;
                //Debug.Log(odor.name+" pungency is "+odor.Pungency);
                tempMat.color = new Color(tempMat.color.r, tempMat.color.g, tempMat.color.b, stank.Pungency);
                stank.HUDMaterial = tempMat;
            }
        }        
    }
}

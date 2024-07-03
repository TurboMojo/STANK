using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace STANK {
public interface ISTANKResponse
    {
        void ProcessThreshold(STANKResponse response);
    }
}
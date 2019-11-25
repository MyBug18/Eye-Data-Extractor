using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;

public class EyeTrackDetector : MonoBehaviour, IGazeFocusable
{
    public bool isGazed;
    public void GazeFocusChanged(bool hasFocus)
    {
        if (hasFocus)
        {
            isGazed = true;
        }
        else
        {
            isGazed = false;
        }
    }
}

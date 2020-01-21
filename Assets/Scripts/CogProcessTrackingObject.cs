using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;

public class CogProcessTrackingObject : MonoBehaviour, IGazeFocusable
{
    private bool isFocused = false;
    public static int TotalCount = 100; // super big number

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocused && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("asdfasdfasdf");
            TotalCount--;
            Destroy(gameObject);
        }
    }

    public void GazeFocusChanged(bool hasFocus)
    {
        isFocused = hasFocus;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationModifier : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    public float speedMultiplier = 1;
    public float baseYrot = 0;

    private float prevY = 0;
    private float currentY = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentY = mainCamera.transform.localEulerAngles.y;
        float deltaY = currentY - prevY;

        if (prevY < 180)
        {
            if (deltaY < 180)
            {

            }
            else
            {
                deltaY -= 360;
            }
        }
        else
        {
            if (-deltaY < 180)
            {

            }
            else
            {
                deltaY += 360;
            }
        }

        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + deltaY * (speedMultiplier - 1), 0);

        prevY = currentY;
    }

    public void SetSpeed(float multiplier)
    {
        baseYrot = transform.localEulerAngles.y; ;
        speedMultiplier = multiplier;
    }
}

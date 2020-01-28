using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraRotationModifier : MonoBehaviour
{
    [SerializeField]
    private Transform mainCamera;

    private bool isDelay = false;
    private float delaySecond = 0;
    public float rotationSpeedMultiplier = 1;
    public float movementSpeedMultiplier = 1;

    private Vector3 prevRotVec = Vector3.zero;
    private Vector3 currentRotVec = Vector3.zero;

    private Vector3 prevPosVec = Vector3.zero;
    private Vector3 currentPosVec = Vector3.zero;

    //private Queue<Vector3> plannedMove = new Queue<Vector3>();

    private float _time = 0;

    private bool initiallized = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            transform.localEulerAngles = Vector3.zero;

        currentRotVec = mainCamera.localEulerAngles;
        Vector3 deltaVec = currentRotVec - prevRotVec;

        if (prevRotVec.y < 180)
        {
            if (deltaVec.y < 180)
            {

            }
            else
            {
                deltaVec.y -= 360;
            }
        }
        else
        {
            if (-deltaVec.y < 180)
            {

            }
            else
            {
                deltaVec.y += 360;
            }
        }

        if (prevRotVec.x < 180)
        {
            if (deltaVec.x < 180)
            {

            }
            else
            {
                deltaVec.x -= 360;
            }
        }
        else
        {
            if (-deltaVec.x < 180)
            {

            }
            else
            {
                deltaVec.x += 360;
            }
        }

        if (prevRotVec.z < 180)
        {
            if (deltaVec.z < 180)
            {

            }
            else
            {
                deltaVec.z -= 360;
            }
        }
        else
        {
            if (-deltaVec.z < 180)
            {

            }
            else
            {
                deltaVec.z += 360;
            }
        }
        transform.localEulerAngles -= deltaVec;

        transform.localEulerAngles += rotationSpeedMultiplier * deltaVec;

        currentPosVec = mainCamera.localPosition;
        Vector3 deltaPos = currentPosVec - prevPosVec;
        transform.localPosition += movementSpeedMultiplier * deltaPos;

        prevRotVec = currentRotVec;
        prevPosVec = currentPosVec;
    }
}

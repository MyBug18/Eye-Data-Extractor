using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraRotationModifier : MonoBehaviour
{
    [SerializeField]
    private Transform mainCamera;

    public bool isDelay = false;
    public float delaySecond = 0;
    public float speedMultiplier = 1;

    private Vector3 prevVec = Vector3.zero;
    private Vector3 currentVec = Vector3.zero;

    private Queue<Vector3> plannedMove = new Queue<Vector3>();

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

        currentVec = mainCamera.localEulerAngles;
        Vector3 deltaVec = currentVec - prevVec;

        if (prevVec.y < 180)
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

        if (prevVec.x < 180)
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

        if (prevVec.z < 180)
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

        if (isDelay)
        {
            if (_time < delaySecond)
            {
                plannedMove.Enqueue(Vector3.zero);
                _time += Time.deltaTime;
            }
            else
            {
                plannedMove.Enqueue(speedMultiplier * deltaVec);
                transform.localEulerAngles += plannedMove.Dequeue();
            }
        }
        else
        {
            transform.localEulerAngles += speedMultiplier * deltaVec;
        }

        prevVec = currentVec;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using System.IO;

public class DataExtractor : MonoBehaviour
{
    protected static EyeData_v2 eyeData = new EyeData_v2();
    protected bool eye_callback_registered = false;

    protected float leftPupilSize;
    protected float rightPupilSize;
    protected Vector2 leftPupilPosition;
    protected Vector2 rightPupilPosition;
    protected string title;
    public string filepath;

    public bool keepWriting = true;

    protected bool leftSizeValid, rightSizeValid, leftPositionValid, rightPositionValid, directionValid;

    protected string leftSize, rightSize, leftPositionX, leftPositionY, rightPositionX, rightPositionY, angleX, angleY;

    protected Vector3 gazeOrigin, gazeDirection;

    protected StreamWriter streamWriter;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }

        File.WriteAllLines(filepath, title.Split('\n'));

        streamWriter = new StreamWriter(filepath, true);      
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }

        if (eye_callback_registered)
        {
            leftPositionValid = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out leftPupilPosition, eyeData);
            rightPositionValid = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out rightPupilPosition, eyeData);
            leftSizeValid = SRanipal_Eye_v2.GetPupilSize(EyeIndex.LEFT, out leftPupilSize, eyeData);
            rightSizeValid = SRanipal_Eye_v2.GetPupilSize(EyeIndex.RIGHT, out rightPupilSize, eyeData);
            directionValid = SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeOrigin, out gazeDirection, eyeData);
        }
        else
        {
            leftPositionValid = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out leftPupilPosition);
            rightPositionValid = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out rightPupilPosition);
            leftSizeValid = SRanipal_Eye_v2.GetPupilSize(EyeIndex.LEFT, out leftPupilSize);
            rightSizeValid = SRanipal_Eye_v2.GetPupilSize(EyeIndex.RIGHT, out rightPupilSize);
            directionValid = SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeOrigin, out gazeDirection);
        }
        if (leftSizeValid) leftSize = leftPupilSize.ToString(); else leftSize = "INVALID";
        if (rightSizeValid) rightSize = rightPupilSize.ToString(); else rightSize = "INVALID";
        if (leftPositionValid)
        {
            leftPositionX = leftPupilPosition.x.ToString();
            leftPositionY = leftPupilPosition.y.ToString();
        }
        else
        {
            leftPositionX = "INVALID";
            leftPositionY = "INVALID";
        }

        if (rightPositionValid)
        {
            rightPositionX = rightPupilPosition.x.ToString();
            rightPositionY = rightPupilPosition.y.ToString();
        }
        else
        {
            rightPositionX = "INVALID";
            rightPositionY = "INVALID";
        }

        if (directionValid)
        {


            float _x = Vector3.Angle(gazeDirection, new Vector3(0, gazeDirection.y, gazeDirection.z));
            float _y = Vector3.Angle(gazeDirection, new Vector3(gazeDirection.x, 0, gazeDirection.z));

            if (gazeDirection.x < 0) _x = -_x;
            if (gazeDirection.y < 0) _y = -_y;

            angleX = _x + "";
            angleY = _y + "";
        }
        else
        {
            angleX = "INVALID";
            angleY = "INVALID";
        }
        /*
        if (keepWriting)
            streamWriter.WriteLine(time + "," + leftSize + "," + rightSize + ", " + leftPosition + ", " + rightPosition);
        */
    }

    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        eyeData = eye_data;
    }
}

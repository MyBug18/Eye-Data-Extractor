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

    public float leftPupilSize;
    public float rightPupilSize;
    public Vector2 leftPupilPosition;
    public Vector2 rightPupilPosition;
    protected string title;
    public string filepath;

    public bool keepWriting = true;

    protected bool leftSizeValid, rightSizeValid, leftPositionValid, rightPositionValid;

    protected string leftSize, rightSize, leftPositionX, leftPositionY, rightPositionX, rightPositionY;

    protected StreamWriter streamWriter;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
        filepath = Application.dataPath + "/ExtractedDatas/data.txt";

        File.WriteAllLines(filepath, title.Split('\n'));

        streamWriter = new StreamWriter(filepath, true);

        
        
        /*
        bool calibrationSuccessful = false;
        do
            calibrationSuccessful = SRanipal_Eye_v2.LaunchEyeCalibration();
        while (!calibrationSuccessful);
        */
        
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
        }
        else
        {
            leftPositionValid = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out leftPupilPosition);
            rightPositionValid = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out rightPupilPosition);
            leftSizeValid = SRanipal_Eye_v2.GetPupilSize(EyeIndex.LEFT, out leftPupilSize);
            rightSizeValid = SRanipal_Eye_v2.GetPupilSize(EyeIndex.RIGHT, out rightPupilSize);
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

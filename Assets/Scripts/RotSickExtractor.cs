using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using System.IO;
public class RotSickExtractor : DataExtractor
{
    [SerializeField]
    private RotSickSceneManager manager;
    float time = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        title = "Time, Head Rotation, Left Pupil Size, Left Pupil Position X, Left Pupil Position Y, Right Pupil Size, Right Pupil Position X, Right Pupil Position Y, Direction X, Direction Y";
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        string result;
        result = time + "," + manager.normalizedCameraRotationY + "," + leftPupilSize + "," + leftPositionX + "," + leftPositionY + "," +
            rightPupilSize + "," + rightPositionX + ", " + rightPositionY + "," + angleX + "," + angleY;

        if (keepWriting)
            streamWriter.WriteLine(result);

        time += Time.deltaTime;
    }
}

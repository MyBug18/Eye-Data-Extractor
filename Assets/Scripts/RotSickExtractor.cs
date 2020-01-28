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
        title = "Time, Head Rotation, Pupil Size, Pupil Position X, Pupil Position Y";
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        string result = time + "," + manager.normalizedCameraRotationY + "," + ((leftSizeValid && rightSizeValid) ? ((leftPupilSize + rightPupilSize) / 2).ToString() : "INVALID") + "," +
            ((leftPositionValid && rightPositionValid) ? ((leftPupilPosition.x + rightPupilPosition.x) / 2).ToString() : "INVALID") + "," +
            ((leftPositionValid && rightPositionValid) ? ((leftPupilPosition.y + rightPupilPosition.y) / 2).ToString() : "INVALID");

        if (keepWriting)
            streamWriter.WriteLine(result);

        time += Time.deltaTime;
    }
}

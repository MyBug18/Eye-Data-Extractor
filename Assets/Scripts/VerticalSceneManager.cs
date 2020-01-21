using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VerticalSceneManager : DataExtractor
{
    [SerializeField]
    public Mode mode;

    [SerializeField]
    private int radius, density;

    [SerializeField]
    private float noise;

    [SerializeField]
    private bool calibrateOnStart;

    [SerializeField]
    private MoveDirection direction;

    [SerializeField]
    private float degreePerSecond;

    [SerializeField]
    private Transform spherePrefab, entireSphereParent, bar, mainCamera;

    [SerializeField]
    private Transform verticalRotator1, verticalRotator2, horizontalRotator1, horizontalRotator2;

    [SerializeField]
    private float initialAngle;

    [SerializeField]
    private bool keepRotating;

    [SerializeField]
    private Transform cam;

    private float time = 0;

    // Start is called before the first frame update

    protected override void Start()
    {
        _InitializeTitleAndPath();

        base.Start();
        _InitializeSpheres();
        _InitializeBar();
        //StartCoroutine(_Rotate());
    }

    protected override void Update()
    {
        base.Update();

        bar.parent.position = mainCamera.position;
        entireSphereParent.position = mainCamera.position;
        if (mode != Mode.SPV)
        {
            if (Input.GetKey(KeyCode.D)) bar.Rotate(0, 0, 5 * Time.deltaTime);
            if (Input.GetKey(KeyCode.A)) bar.Rotate(0, 0, -5 * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            string _result = "";
            _result += "Direction: " + direction;
            _result += "\nDegree per Second: " + degreePerSecond;
            _result += "\nInitial Degree: " + initialAngle;

            float finalDegree;
            if (mode == Mode.SPV) finalDegree = cam.transform.localEulerAngles.z;
            else finalDegree = bar.localEulerAngles.z;

            _result += "\nFinal Degree: " + _NormalizeAngle(finalDegree);

            File.WriteAllLines("./Assets/ExtractedDatas/Subject_things/" + mode + "_info.txt", _result.Split('\n'));
            keepWriting = false;
        }

        switch (direction)
        {
            case MoveDirection.Vertical:
                entireSphereParent.Rotate(new Vector3(degreePerSecond * Time.deltaTime, 0, 0));
                break;
            case MoveDirection.Horizontal:
                entireSphereParent.Rotate(new Vector3(0, degreePerSecond * Time.deltaTime, 0));
                break;
            case MoveDirection.Diagonal:
                entireSphereParent.Rotate(new Vector3(degreePerSecond * Time.deltaTime, degreePerSecond * Time.deltaTime, 0));
                break;
            case MoveDirection.Clockwise:
                entireSphereParent.Rotate(new Vector3(0, 0, -degreePerSecond * Time.deltaTime));
                break;
            case MoveDirection.CounterClockwise:
                entireSphereParent.Rotate(new Vector3(0, 0, degreePerSecond * Time.deltaTime));
                break;
            case MoveDirection.VerticalZigZag:
                verticalRotator1.Rotate(degreePerSecond * Time.deltaTime, 0, 0);
                verticalRotator2.Rotate(-degreePerSecond * Time.deltaTime, 0, 0);
                break;
            case MoveDirection.HorizontalZigZag:
                horizontalRotator1.Rotate(0, degreePerSecond * Time.deltaTime, 0);
                horizontalRotator2.Rotate(0, -degreePerSecond * Time.deltaTime, 0);
                break;
            default:
                throw new System.InvalidOperationException("Invalid Move Mode Enum!");
        }

        if (keepWriting)
        {
            string result = time + "," + ((leftSizeValid && rightSizeValid) ? ((leftPupilSize + rightPupilSize) / 2).ToString() : "INVALID");
            streamWriter.WriteLine(result);
            time += Time.deltaTime;
        }
    }

    private void _InitializeSpheres()
    {

        float scaling = radius;
        Vector3[] pts = _PointsOnSphere(density * radius);
        List<GameObject> uspheres = new List<GameObject>();
        int i = 0;

        foreach (Vector3 value in pts)
        {
            var s = Instantiate(spherePrefab);
            uspheres.Add(s.gameObject);
            if (direction == MoveDirection.VerticalZigZag)
            {
                if (value.x > 0) s.parent = verticalRotator1;
                else s.parent = verticalRotator2;
            }
            else if (direction == MoveDirection.HorizontalZigZag)
            {
                if (value.y > 0) s.parent = horizontalRotator1;
                else s.parent = horizontalRotator2;
            }
            else
            {
                s.parent = entireSphereParent;
            }
            uspheres[i].transform.position = value * scaling;
            i++;
        }
    }

    private Vector3[] _PointsOnSphere(int n)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;
        float _noise = noise * radius;

        for (var k = 0; k < n; k++)
        {
            y = k * off - 1 + (off / 2) * Random.Range(1 - _noise, 1 + _noise);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r * Random.Range(1 - _noise, 1 + _noise);
            z = Mathf.Sin(phi) * r * Random.Range(1 - _noise, 1 + _noise);

            upts.Add(new Vector3(x, y, z));
        }
        Vector3[] pts = upts.ToArray();
        return pts;
    }

    private IEnumerator _Rotate()
    {
        while (true)
        {
            if (keepRotating)
            {
                float deg = 0;
                MoveDirection dir = direction;

                while (deg < 360)
                {
                    float amount = Time.deltaTime * degreePerSecond;
                    deg += amount;
                    switch(dir)
                    {
                        case MoveDirection.Horizontal:
                            entireSphereParent.Rotate(new Vector3(0, amount, 0));
                            break;
                        case MoveDirection.Vertical:
                            entireSphereParent.Rotate(new Vector3(amount, 0, 0));
                            break;
                        case MoveDirection.Diagonal:
                            entireSphereParent.Rotate(new Vector3(amount, amount, 0));
                            break;
                        case MoveDirection.Clockwise:
                            entireSphereParent.Rotate(new Vector3(0, 0, -amount));
                            break;
                        case MoveDirection.CounterClockwise:
                            entireSphereParent.Rotate(new Vector3(0, 0, amount));
                            break;
                        default:
                            throw new System.InvalidOperationException("Invalid Enum Type!");

                    }
                    Debug.Log(deg);
                    yield return null;
                }
            }
            yield return null;
        }
    }

    private void _InitializeBar()
    {
        bar.Rotate(0, 0, initialAngle);
        if (mode == Mode.SVV) bar.parent.Rotate(0, 0, 90);
    }

    private void _InitializeTitleAndPath()
    {
        title = "Time, Pupil Size";
        switch(mode)
        {
            case Mode.SVH:
                filepath = "./Assets/ExtractedDatas/Subject_things/SVH_data.txt";
                break;
            case Mode.SVV:
                filepath = "./Assets/ExtractedDatas/Subject_things/SVV_data.txt";
                break;
            case Mode.SPV:
                filepath = "./Assets/ExtractedDatas/Subject_things/SPV_data.txt";
                break;
        }
    }

    private float _NormalizeAngle(float v)
    {
        return v > 180 ? v - 360 : v;
    }
}

public enum MoveDirection
{
    Vertical,
    Horizontal,
    Diagonal,
    Clockwise,
    CounterClockwise,
    VerticalZigZag,
    HorizontalZigZag
}

public enum Mode
{
    SVH,
    SVV,
    SPV
}
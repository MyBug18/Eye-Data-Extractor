﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSceneManager : DataExtractor
{
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
    private Transform spherePrefab, sphereParent, bar, mainCamera;


    [SerializeField]
    private float initialAngle;

    [SerializeField]
    private bool keepRotating;

    // Start is called before the first frame update

    protected override void Start()
    {
        title = "asdf";
        base.Start();
        _InitializeSpheres();
        _InitializeBar();
        StartCoroutine(_Rotate());
    }

    protected override void Update()
    {
        base.Update();

        bar.parent.position = mainCamera.position;
        sphereParent.position = mainCamera.position;

        if (Input.GetKey(KeyCode.D)) bar.Rotate(0, 0, 5 * Time.deltaTime);

        if (Input.GetKey(KeyCode.A)) bar.Rotate(0, 0, -5 * Time.deltaTime);
    }

    private void _InitializeSpheres()
    {

        float scaling = radius;
        Vector3[] pts = _PointsOnSphere(density * radius);
        List<GameObject> uspheres = new List<GameObject>();
        int i = 0;

        foreach (Vector3 value in pts)
        {
            uspheres.Add(Instantiate(spherePrefab, sphereParent).gameObject);
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
                Debug.Log("asdf");
                float deg = 0;
                MoveDirection dir = direction;

                while (deg < 360)
                {
                    float amount = Time.deltaTime * degreePerSecond;
                    deg += amount;
                    switch(dir)
                    {
                        case MoveDirection.Horizontal:
                            sphereParent.Rotate(new Vector3(0, amount, 0));
                            break;
                        case MoveDirection.Vertical:
                            sphereParent.Rotate(new Vector3(amount, 0, 0));
                            break;
                        case MoveDirection.Diagonal:
                            sphereParent.Rotate(new Vector3(amount, amount, 0));
                            break;
                        case MoveDirection.Clockwise:
                            sphereParent.Rotate(new Vector3(0, 0, -amount));
                            break;
                        case MoveDirection.CounterClockwise:
                            sphereParent.Rotate(new Vector3(0, 0, amount));
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
    }
}

public enum MoveDirection
{
    Vertical,
    Horizontal,
    Diagonal,
    Clockwise,
    CounterClockwise
}

public enum Mode
{
    SVV,
    SPV
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ViveSR.anipal.Eye;

public class RotSickSceneManager : MonoBehaviour
{
    public bool calibrateOnStart = false;
    public bool withGrid = true;
    public int angleLimit = 30;
    public float rotationSpeedMultiplier = 1;
    public float movementSpeedMultiplier = 1;

    [SerializeField]
    private GameObject buildings;

    [SerializeField]
    private CameraRotationModifier crm;

    [SerializeField]
    private EyeTrackDetector detectorObject;

    [SerializeField]
    private TextMeshPro tm;

    [SerializeField]
    private Transform lineRotator, headRotator;

    [SerializeField]
    private Transform gridPrefab, gridParent;

    private Color c1, c2;

    [SerializeField]
    private Camera mainCamera;

    private float rotDifferenceBound = 5;
    private float animationTime = 0.1f;

    private float normalizedLineRotatorY => lineRotator.localEulerAngles.y > 180 ? lineRotator.localEulerAngles.y - 360 : lineRotator.localEulerAngles.y;

    public float normalizedCameraRotationY => mainCamera.transform.eulerAngles.y > 180 ? mainCamera.transform.eulerAngles.y - 360 : mainCamera.transform.eulerAngles.y;

    private bool isHeadRotTracked = false;


    // Start is called before the first frame update
    void Start()
    {

        if (calibrateOnStart)
        {
            bool calibrationSuccessful = false;
            do
                calibrationSuccessful = SRanipal_Eye_v2.LaunchEyeCalibration();
            while (!calibrationSuccessful);
        }

        StartTesting();
        if (withGrid)
            _InitiallizeGrid();
        else
            buildings.SetActive(true);
        c1 = lineRotator.GetChild(0).GetComponent<Renderer>().material.color;
        c2 = Color.blue;

        crm.rotationSpeedMultiplier = rotationSpeedMultiplier;
        crm.movementSpeedMultiplier = movementSpeedMultiplier;

        string info = "";
        info += "With Building: " + !withGrid;
        info += "\nRotation Speed Multiplier: " + rotationSpeedMultiplier;
        info += "\nMovement Speed Multiplier: " + movementSpeedMultiplier;

        System.IO.File.WriteAllLines("Assets/ExtractedDatas/RotSickness/info.txt", info.Split('\n'));
    }

    // Update is called once per frame
    void Update()
    {
        if (headRotator.gameObject.activeSelf)
        {
            if (Mathf.Abs(normalizedLineRotatorY - normalizedCameraRotationY) < rotDifferenceBound)
            {
                c2 = Color.blue;
                isHeadRotTracked = true;
            }
            else
            {
                c2 = c1;
                isHeadRotTracked = false;
            }
        }

        lineRotator.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(lineRotator.GetChild(0).GetComponent<Renderer>().material.color, c2, Time.deltaTime * (1 / animationTime));
    }

    private void _InitiallizeGrid()
    {
        for (int i = -90; i <= 90; i += 10)
        {
            var grid = Instantiate(gridPrefab, gridParent);
            grid.localPosition = new Vector3(0, 0, 0);
            grid.localEulerAngles = new Vector3(0, i, 0);
        }
    }
    public void StartTesting()
    {
        StartCoroutine(_LineRotatorMovement());
    }

    private IEnumerator _LineRotatorMovement()
    {
        while (true)
        {
            while (normalizedLineRotatorY < angleLimit)
            {
                lineRotator.transform.localEulerAngles += new Vector3(0, 5 * Time.deltaTime, 0);
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            while (normalizedLineRotatorY > -angleLimit)
            {
                lineRotator.transform.localEulerAngles += new Vector3(0, -5 * Time.deltaTime, 0);
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }

}

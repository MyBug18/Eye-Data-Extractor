using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ViveSR.anipal.Eye;

public class RotSickSceneManager : MonoBehaviour
{

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

    public int angleLimit = 30;

    // Start is called before the first frame update
    void Start()
    {
        StartTesting();
        _InitiallizeGrid();
        c1 = lineRotator.GetChild(0).GetComponent<Renderer>().material.color;
        c2 = Color.blue;        
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
        headRotator.position = mainCamera.transform.position;
        gridParent.position = mainCamera.transform.position;
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
        StartCoroutine(_BeforeFirstGazeTest());
    }


    private IEnumerator _BeforeFirstGazeTest()
    {
        /*
        tm.text = "안녕하세요.";
        yield return new WaitForSeconds(5f);
        tm.text = "3초간 정면의 큐브를 봐 주시기 바랍니다.";
        */
        yield return new WaitForSeconds(1f);
        StartCoroutine(_FirstGazeTest());
    }

    private IEnumerator _FirstGazeTest()
    {
        detectorObject.gameObject.SetActive(true);
        float time = 0;
        float loseTime = 0;
        while (time < 3)
        {
            time += Time.deltaTime;
            if (detectorObject.isGazed == false)
            {
                loseTime += Time.deltaTime;
            }
            else
            {
                loseTime = 0;
            }

            if (loseTime > 0.5)
            {
                tm.text = "다시 큐브를 봐 주시기 바랍니다.";
                time = 0;
            }
            yield return null;
        }
        tm.text = "잘 하셨습니다.";
        //detectorObject.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        StartCoroutine(_HeadRotationTest());
    }

    private IEnumerator _HeadRotationTest()
    {
        headRotator.gameObject.SetActive(true);
        tm.text = "이제 왼쪽의 하얀 막대기가\n오른쪽으로 움직입니다.";
        yield return new WaitForSeconds(2f);
        tm.text = "막대가 중심의 큐브를 지나칠 때부터";
        yield return new WaitForSeconds(2f);
        tm.text = "큐브를 계속 쳐다보며";
        yield return new WaitForSeconds(2f);
        tm.text = "막대를 따라 고개를\n돌려 주시기 바랍니다.";
        yield return new WaitForSeconds(2f);
        tm.text = "고개가 옳은 방향을 향하고 있다면";
        yield return new WaitForSeconds(2f);
        tm.text = "막대가 파랗게 변할 것입니다.";

        yield return StartCoroutine(_LineRotatorMovement());
    }

    private IEnumerator _LineRotatorMovement()
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

        while (normalizedLineRotatorY < 0)
        {
            lineRotator.transform.localEulerAngles += new Vector3(0, 5 * Time.deltaTime, 0);
            yield return null;
        }
    }

}

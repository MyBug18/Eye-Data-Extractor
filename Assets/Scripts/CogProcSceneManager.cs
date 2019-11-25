using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogProcSceneManager : MonoBehaviour
{
    [SerializeField]
    private Transform gridPrefab, gridParent;

    [SerializeField]
    private Transform cubePrefab, cubeParent;

    private Transform[,] cubeGridArray = new Transform[32, 32];

    //[SerializeField]
    //private CameraBlur blurrer;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = -16; i <= 16; i++)
        {
            Instantiate(gridPrefab, new Vector3((float)i * 2 / 3, 0, 20), Quaternion.identity, gridParent);

            var gm = Instantiate(gridPrefab, new Vector3(0, (float)i * 2 / 3, 20), Quaternion.identity, gridParent);
            gm.localEulerAngles = new Vector3(0, 0, 90);
        }

        
        for (int i = 0; i < 32; i++)
            for (int j = 0; j < 32; j++)
            {
                cubeGridArray[i, j] = Instantiate(cubePrefab, _GridCoordToPos(i, j), Quaternion.identity, cubeParent);
            }

        var tf = new bool[] { true, false };

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                SetGrid4x4Element(i * 4, j * 4, tf[Random.Range(0, 1)]);
            }
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Q))
            blurrer.blurMat.SetFloat("_BlurSize", 0f);

        if (Input.GetKeyDown(KeyCode.W))
            blurrer.blurMat.SetFloat("_BlurSize", 0.08f);
            */
    }

    private bool _IsAdjacent(Vector2Int v1, Vector2Int v2)
    {
        Vector2Int diff = v1 - v2;
        if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1) return true;
        else return false;
    }

    private Vector3 _GridCoordToPos(int x, int y)
    {
        return new Vector3(-31f / 3 + (2 * (float)x / 3), -31f / 3 + (2 * (float)y / 3), 20);
    }

    private void SetGrid4Element(float colorH, int startX, int startY, bool isHorizontal)
    {
        float[] colorS = new float[] { 25f, 50f, 75f, 100f };
        int n = colorS.Length;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            float temp = colorS[n];
            colorS[n] = colorS[k];
            colorS[k] = temp;
        }

        if (isHorizontal)
        {
            for (int i = 0; i < 4; i++)
            {
                cubeGridArray[startX + i, startY].GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(colorH / 360, colorS[i] / 100, 1);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                cubeGridArray[startX, startY + i].GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(colorH / 360, colorS[i] / 100, 1);
            }
        }

    }

    private void SetGrid4x4Element(int leftX, int downY, bool isHorizontal)
    {

        float[] colorH = new float[] { 0f, 300f, 120f, 225f };
        int n = colorH.Length;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            float temp = colorH[n];
            colorH[n] = colorH[k];
            colorH[k] = temp;
        }
        if (isHorizontal)
        {
            for (int i = 0; i < 4; i++)
            {
                SetGrid4Element(colorH[i], leftX, downY + i, isHorizontal);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                SetGrid4Element(colorH[i], leftX + i, downY, isHorizontal);
            }
        }
    }
}

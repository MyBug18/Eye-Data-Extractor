using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogProcSceneManager : MonoBehaviour
{
    [SerializeField]
    private Transform trackingObjectPrefab;

    [SerializeField]
    private Transform gridPrefab, gridParent;

    [SerializeField]
    private Transform cubePrefab, cubeParent, edgeParent;

    private Transform[,] cubeGridArray = new Transform[32, 32];

    [SerializeField]
    private MoveSpeed backGroundMoveSpeed, trackingObjectMoveSpeed;

    [SerializeField]
    private int trackingObjectNumber;

    [SerializeField]
    private MoveMode mode;
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
                cubeGridArray[i, j].name = "Cube";
            }

        var tf = new bool[] { true, false };

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                SetGrid4x4Element(i * 4, j * 4, tf[Random.Range(0, 1)]);
            }

        _InstantiateEdges();
        _StartMoving();

        float zoffset = 0.1f;

        for (int i = 0; i < trackingObjectNumber; i++)
        {
            var o = Instantiate(trackingObjectPrefab, _GetRandomStartingPosition(), Quaternion.identity);
            o.Translate(0, 0, -zoffset);
            zoffset -= 0.1f;
            StartCoroutine(_MoveOneTrackingObject(o));
        }
    }

    private void _InstantiateEdges()
    {
        for (int i = -1; i < 32; i++)
        {
            Instantiate(cubePrefab, _GridCoordToPos(-1, i) + new Vector3(0, 0, -0.03f), Quaternion.identity, edgeParent);
            Instantiate(cubePrefab, _GridCoordToPos(32, i + 1) + new Vector3(0, 0, -0.03f), Quaternion.identity, edgeParent);
            Instantiate(cubePrefab, _GridCoordToPos(i + 1, -1) + new Vector3(0, 0, -0.03f), Quaternion.identity, edgeParent);
            Instantiate(cubePrefab, _GridCoordToPos(i, 32) + new Vector3(0, 0, -0.03f), Quaternion.identity, edgeParent);
        }

        for (int i = -2; i < 33; i++)
        {
            Instantiate(cubePrefab, _GridCoordToPos(-2, i), Quaternion.identity, edgeParent);
            Instantiate(cubePrefab, _GridCoordToPos(33, i + 1), Quaternion.identity, edgeParent);
            Instantiate(cubePrefab, _GridCoordToPos(i + 1, -2), Quaternion.identity, edgeParent);
            Instantiate(cubePrefab, _GridCoordToPos(i, 33), Quaternion.identity, edgeParent);
        }
    }

    private Vector3 _GetRandomStartingPosition()
    {
        int x = Random.Range(-1, 2) * 5;
        int y = Random.Range(-1, 2) * 5;
        float z = 19.5f;
        return new Vector3(x, y, z);
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

    private enum _Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    public enum MoveMode
    {
        Stop,
        Up,
        Down,
        Left,
        Right,
        VerticalZigZag,
        HorizontalZigZag
    }

    public enum MoveSpeed
    {
        Slow = 1,
        Moderate = 2,
        Fast = 3
    }

    private void _StartMoving()
    {
        switch (mode)
        {
            case MoveMode.Stop:
                break;
            case MoveMode.Up:
                for (int i = 0; i < 8; i++)
                    _Move4Line(_Direction.Up, i);
                break;
            case MoveMode.Down:
                for (int i = 0; i < 8; i++)
                    _Move4Line(_Direction.Down, i);
                break;
            case MoveMode.Left:
                for (int i = 0; i < 8; i++)
                    _Move4Line(_Direction.Left, i);
                break;
            case MoveMode.Right:
                for (int i = 0; i < 8; i++)
                    _Move4Line(_Direction.Right, i);
                break;
            case MoveMode.VerticalZigZag:
                for (int i = 0; i < 8; i++)
                    if (i % 2 == 0) _Move4Line(_Direction.Up, i); else _Move4Line(_Direction.Down, i);
                break;
            case MoveMode.HorizontalZigZag:
                for (int i = 0; i < 8; i++)
                    if (i % 2 == 0) _Move4Line(_Direction.Right, i); else _Move4Line(_Direction.Left, i);
                break;

            default:
                throw new System.Exception();
        }
    }

    private void _Move4Line(_Direction dir, int chunkNum)
    {
        for (int i = 0; i < 4; i++)
            StartCoroutine(_MoveOneLineOnceEnum(dir, 4 * chunkNum + i));
    }

    private IEnumerator _MoveOneLineOnceEnum(_Direction dir, int lineNum)
    {
        while (true)
        {
            float amount = 0;
            switch (dir)
            {
                case _Direction.Left:
                    var aux1 = Instantiate(cubeGridArray[0, lineNum], _GridCoordToPos(32, lineNum), Quaternion.identity, cubeParent);
                    aux1.name = "Cube";
                    while (aux1.position.x - _GridCoordToPos(31, lineNum).x > 0)
                    {
                        amount += Time.fixedDeltaTime * (int)backGroundMoveSpeed;
                        for (int i = 0; i < 32; i++)
                            cubeGridArray[i, lineNum].Translate(-Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0, 0);
                        aux1.Translate(-Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0, 0);

                        yield return null;
                    }

                    Destroy(cubeGridArray[0, lineNum].gameObject);
                    for (int i = 0; i < 31; i++)
                        cubeGridArray[i, lineNum] = cubeGridArray[i + 1, lineNum];
                    cubeGridArray[31, lineNum] = aux1;
                    break;

                case _Direction.Right:
                    var aux2 = Instantiate(cubeGridArray[31, lineNum], _GridCoordToPos(-1, lineNum), Quaternion.identity, cubeParent);
                    aux2.name = "Cube";
                    while (aux2.position.x - _GridCoordToPos(0, lineNum).x < 0)
                    {
                        amount += Time.fixedDeltaTime * (int)backGroundMoveSpeed;
                        for (int i = 0; i < 32; i++)
                            cubeGridArray[i, lineNum].Translate(Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0, 0);
                        aux2.Translate(Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0, 0);

                        yield return null;
                    }

                    Destroy(cubeGridArray[31, lineNum].gameObject);
                    for (int i = 31; i > 0; i--)
                        cubeGridArray[i, lineNum] = cubeGridArray[i - 1, lineNum];
                    cubeGridArray[0, lineNum] = aux2;
                    break;

                case _Direction.Up:
                    var aux3 = Instantiate(cubeGridArray[lineNum, 31], _GridCoordToPos(lineNum, -1), Quaternion.identity, cubeParent);
                    aux3.name = "Cube";
                    while (aux3.position.y - _GridCoordToPos(lineNum, 0).y < 0)
                    {
                        amount += Time.fixedDeltaTime * (int)backGroundMoveSpeed;
                        for (int i = 0; i < 32; i++)
                            cubeGridArray[lineNum, i].Translate(0, Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0);
                        aux3.Translate(0, Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0);

                        yield return null;
                    }

                    Destroy(cubeGridArray[lineNum, 31].gameObject);
                    for (int i = 31; i > 0; i--)
                        cubeGridArray[lineNum, i] = cubeGridArray[lineNum, i - 1];
                    cubeGridArray[lineNum, 0] = aux3;
                    break;

                case _Direction.Down:
                    var aux4 = Instantiate(cubeGridArray[lineNum, 0], _GridCoordToPos(lineNum, 32), Quaternion.identity, cubeParent);
                    aux4.name = "Cube";
                    while (aux4.position.y - _GridCoordToPos(lineNum, 31).y > 0)
                    {
                        amount += Time.fixedDeltaTime * (int)backGroundMoveSpeed;
                        for (int i = 0; i < 32; i++)
                            cubeGridArray[lineNum, i].Translate(0, -Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0);
                        aux4.Translate(0, -Time.fixedDeltaTime * (int)backGroundMoveSpeed, 0);

                        yield return null;
                    }

                    Destroy(cubeGridArray[lineNum, 0].gameObject);
                    for (int i = 0; i < 31; i++)
                        cubeGridArray[lineNum, i] = cubeGridArray[lineNum, i + 1];
                    cubeGridArray[lineNum, 31] = aux4;
                    break;

                default:
                    throw new System.Exception();
            }
            
        }
    }

    private bool _IsInBoundWithMargin(Vector3 position)
    {
        float left = _GridCoordToPos(5, 5).x;
        float right = _GridCoordToPos(26, 26).x;

        float up = _GridCoordToPos(26, 26).y;
        float down = _GridCoordToPos(5, 5).y;

        Debug.Log(position.x > left && position.x < right && position.y > down && position.y < up);
        return position.x > left && position.x < right && position.y > down && position.y < up;
    }

    private IEnumerator _MoveOneTrackingObject(Transform o)
    {
        while(true)
        {
            float counter = 0;
            Vector3 startPos = o.position;
            Vector3 toPosition = startPos;
            float amount = Random.Range(0, 5f);
            float duration = amount / ((float)trackingObjectMoveSpeed * 1.5f);

            switch((_Direction)Random.Range(0, 4))
            {
                case _Direction.Down:
                    toPosition.y -= amount;
                    break;
                case _Direction.Left:
                    toPosition.x -= amount;
                    break;
                case _Direction.Right:
                    toPosition.x += amount;
                    break;
                case _Direction.Up:
                    toPosition.y += amount;
                    break;

            }

            if (!_IsInBoundWithMargin(toPosition)) continue;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                o.position = Vector3.Lerp(startPos, toPosition, counter / duration);
                yield return null;
            }

        }
    }

    /*
    private void _MoveOneLineChunk(_Direction dir, int chunkNum)
    {
        for (int i = 0; i < 4; i++)
            _MoveOneSmallLine(dir, 4 * chunkNum + i);
    }


    private void _MoveOneSmallLine(_Direction dir, int lineNum) // 0_based
    {
        switch(dir)
        {
            case _Direction.Left:
                var tmp1 = cubeGridArray[0, lineNum];
                for (int i = 1; i < 32; i++)
                    cubeGridArray[i - 1, lineNum] = cubeGridArray[i, lineNum];
                cubeGridArray[31, lineNum] = tmp1;

                for (int i = 0; i < 32; i++)
                    cubeGridArray[i, lineNum].position = _GridCoordToPos(i, lineNum);
                break;

            case _Direction.Right:
                var tmp2 = cubeGridArray[31, lineNum];
                for (int i = 31; i > 0; i--)
                    cubeGridArray[i, lineNum] = cubeGridArray[i - 1, lineNum];
                cubeGridArray[0, lineNum] = tmp2;

                for (int i = 0; i < 32; i++)
                    cubeGridArray[i, lineNum].position = _GridCoordToPos(i, lineNum);
                break;

            case _Direction.Up:
                var tmp3 = cubeGridArray[lineNum, 31];
                for (int i = 31; i > 0; i--)
                    cubeGridArray[lineNum, i] = cubeGridArray[lineNum, i - 1];
                cubeGridArray[lineNum, 0] = tmp3;

                for (int i = 0; i < 32; i++)
                    cubeGridArray[lineNum, i].position = _GridCoordToPos(lineNum, i);
                break;

            case _Direction.Down:
                var tmp4 = cubeGridArray[lineNum, 0];
                for (int i = 1; i < 32; i++)
                    cubeGridArray[lineNum, i - 1] = cubeGridArray[lineNum, i];
                cubeGridArray[lineNum, 31] = tmp4;

                for (int i = 0; i < 32; i++)
                    cubeGridArray[lineNum, i].position = _GridCoordToPos(lineNum, i);
                break;
        }
    }
    */
}

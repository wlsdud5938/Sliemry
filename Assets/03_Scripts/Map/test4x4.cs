using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class test4x4 : MonoBehaviour
{
    class branch
    {
        branch(int maxRoom)
        {
            branchMaxRoom = maxRoom;
        }
        int branchMaxRoom;
        public Queue<int[]> roomPos;
        int curRoomCount = 0;
    }

    public GameObject start;
    public GameObject end;
    public GameObject cube;

    int x1;
    int x2;
    int x;
    int y;
    int y1;
    int y2;

    public int branchMaxRoom = 4;
    public int startNode;
    public int endNode;

    List<int[]> path = new List<int[]>();

    int[,] matrix = { { -1, -1, -1, -1}, { -1, -1, -1, -1}, { -1, -1, -1, -1}, { -1, -1, -1, -1 }, { -1, -1, -1, -1 } };
    int[,] doorMatrix = { { 0, 0, 0, 0}, { 0, 0, 0, 0}, { 0, 0, 0, 0}, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };

    int[,] realMatrix = new int[14, 14];
    int pathSize = 0;
    int min = 5;

    Queue<int[]> openRoom = new Queue<int[]>();

    int[,] ta = new int[20, 20];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (i - 1 == j || i + 1 == j || i + 5 == j || i - 5 == j)
                    ta[i, j] = 1;
                else
                    ta[i, j] = 0;
                if ((i + 1 == j && i % 5 == 4) || (i - 1 == j && i % 5 == 0))
                    ta[i, j] = 0;
            }
        }

        x1 = Random.Range(0, 4);
        x2 = Random.Range(0, 4);
        y1 = Random.Range(0, 4);
        y2 = Random.Range(0, 4);
        if (x1 == x2 && y1 == y2)
        {
            if (y2 != 4)
                y2++;
            else
                y2--;
        }
        x = x1;
        y = y1;
        matrix[x1, y1] = 1;
        matrix[x2, y2] = 2;
        start.transform.position = new Vector3(x1, y1);
        end.transform.position = new Vector3(x2, y2);
        DFSAlgorithm d = new DFSAlgorithm();
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (ta[i, j] == 1)
                    d.inputData(i, j);
            }
        }
        startNode = x1 + y1 * 5;
        endNode = x2 + y2 * 5;
        d.DFS(startNode, endNode, path);
        int ran = Random.Range(0, path.Count);

        string st = "";
        for(int i=0;i< path[ran].Length;i++)
        {
            st += path[ran][i].ToString() + " ";
        }
        Debug.Log(st);
        Debug.Log(matrix);

        for (int j = 0; j < path[ran].Length; j++)
        {
            if (j != 0 && j != path[ran].Length - 1)
            {
                Vector2 v = new Vector2(path[ran][j] % 5, path[ran][j] / 5);
                matrix[(int)v.x, (int)v.y] = 0;
                Debug.Log(v.x.ToString() + " " + v.y.ToString());

                CheckTBLR((int)v.x, (int)v.y);
                x = path[ran][j] % 5 - x2;
                y = path[ran][j] / 5 - y2;
                //Debug.Log(x.ToString() + " " + y.ToString());
                x2 += x;
                y2 += y;
                Instantiate(cube, v, Quaternion.identity);
            }
        }

        MatrixMoveToReal();
        for(int i =0;i<openRoom.Count;i++)
        {
            int[] pos = openRoom.Dequeue();
            OpenTheDoor(pos[0], pos[1]);
            CloseTheDoor(pos[0], pos[1]);
        }
    }


    void CheckTBLR(int a, int b)
    {
        matrix[a, b] = 0;
        if (b < 3 && matrix[a, b + 1] != -1)
        {
            doorMatrix[a, b] |= 8;
            doorMatrix[a, b + 1] |= 4;
        }
        if (b > 0 && matrix[a, b - 1] != -1)
        {
            doorMatrix[a, b] |= 4;
            doorMatrix[a, b - 1] |= 8;
        }
        if (a > 0 && matrix[a - 1, b] != -1)
        {
            doorMatrix[a, b] |= 2;
            doorMatrix[a - 1, b] |= 1;
        }
        if (a < 4 && matrix[a + 1, b] != -1)
        {
            doorMatrix[a, b] |= 1;
            doorMatrix[a + 1, b] |= 2;
        }
    }

    void OpenTheDoor(int a, int b)
    {
        if (realMatrix[a, b + 1] == 0)
        {
            int ran = Random.Range(0, 1);
            ran = ran << 3;
            realMatrix[a, b] |= ran;
            ran = ran >> 1;
            realMatrix[a, b + 1] |= ran;
        }
        if (realMatrix[a, b - 1] == 0)
        {
            int ran = Random.Range(0, 1);
            ran = ran << 2;
            realMatrix[a, b] |= ran;
            ran = ran << 1;
            realMatrix[a, b - 1] |= ran;
        }
        if (realMatrix[a - 1, b] == 0)
        {
            int ran = Random.Range(0, 1);
            ran = ran << 1;
            realMatrix[a, b] |= ran;
            ran = ran >> 1;
            realMatrix[a - 1, b] |= ran;
        }
        if (realMatrix[a + 1, b] == 0)
        {
            int ran = Random.Range(0, 1);
            realMatrix[a, b] |= ran;
            ran = ran << 1;
            realMatrix[a + 1, b] |= ran;
        }
    }
    void CloseTheDoor(int a, int b)
    {
        if ((realMatrix[a, b + 1] & 4) != 1)
        {
            realMatrix[a, b] = realMatrix[a, b] | 8;
            realMatrix[a, b] -= 8;
        }
        if ((realMatrix[a, b - 1] & 8) != 1)
        {
            realMatrix[a, b] = realMatrix[a, b] | 4;
            realMatrix[a, b] -= 4;
        }
        if ((realMatrix[a + 1, b] & 2) != 1)
        {
            realMatrix[a, b] = realMatrix[a, b] | 1;
            realMatrix[a, b] -= 1;
        }
        if ((realMatrix[a - 1, b] & 1) != 1)
        {
            realMatrix[a, b] = realMatrix[a, b] | 2;
            realMatrix[a, b] -= 2;
        }
    }

    void MatrixMoveToReal()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (doorMatrix[i, j] != 0)
                {
                    realMatrix[i + 5, j + 5] = doorMatrix[i, j];
                    if (matrix[i, j] == 0)
                    {
                        int[] enq = { i + 5, j + 5 };
                        openRoom.Enqueue(enq);
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class test4x4 : MonoBehaviour
{
    public GameObject start;
    public GameObject end;
    public GameObject cube;

    int x1;
    int x2;
    int x;
    int y;
    int y1;
    int y2;


    public int startNode;
    public int endNode;

    List<int[]> path = new List<int[]>();

    int pathSize = 0;
    int min = 5;

    int[,] ta = new int[16,16];

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<16;i++)
        {
            for(int j=0;j<16;j++)
            {
                if(i - 1 == j || i + 1 == j || i + 4 == j || i - 4 == j)
                    ta[i, j] = 1;
                else
                    ta[i, j] = 0;
                if((i + 1 == j && i%4==3) || (i - 1 == j && i % 4 == 0))
                    ta[i, j] = 0;
            }
        }

        x1 = Random.Range(0, 3);
        x2 = Random.Range(0, 3);
        y1 = Random.Range(0, 3);
        y2 = Random.Range(0, 3);
        if (x1 == x2 && y1 == y2)
        {
            if (y2 != 3)
                y2++;
            else 
                y2--;
        }
        x = x1;
        y = y1;
        start.transform.position = new Vector3(x1, y1);
        end.transform.position = new Vector3(x2, y2);
        DFSAlgorithm d = new DFSAlgorithm();
        for(int i=0;i<16;i++)
        {
            for (int j = 0; j<i;j++)
            {
                if(ta[i,j] ==1)
                    d.inputData(i, j);
            }
        }
        startNode = x1 + y1 * 4;
        endNode = x2 + y2 * 4;
        d.DFS(startNode,endNode,path);
        int ran = Random.Range(0, path.Count);
        for(int j=0;j<path[ran].Length;j++)
        {
            if (j != 0 && j != path[ran].Length - 1)
            {
                Vector2 v = new Vector2(path[ran][j] % 4, path[ran][j] / 4);
                Instantiate(cube, v , Quaternion.identity);
            }
        }

    }
    

    bool CheckStartEnd()
    {
        if(x1==x2&&y1==y2)
            return true;
        return false;
    }
    
    
}

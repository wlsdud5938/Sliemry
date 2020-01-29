using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFSAlgorithm : MonoBehaviour
{
    private int[,] maps = new int[20, 20];      //DFS 인접행렬
    private bool[] visit = new bool[20];   //방문했나 안했나 판단할 변수
    Stack<int> stack = new Stack<int>();

    public DFSAlgorithm()
    {
        //클래스 생성자
        //스택을 초기화하고
        //table 및  visit 변수를 할당 한다.

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                maps[i, j] = 0;
            }
        }

        for (int i = 0; i < 20; i++)
        {
            visit[i] = false;
        }
    }

    public void inputData(int i, int j)
    {
        //데이터를 집어넣는 함수
        //i,j를 넣으면 인접행렬에 값을 넣어준다.
        //무방향 그래프이므로 대칭해서 넣어준다.
        maps[i, j] = 1;
        maps[j, i] = 1;

    }
    public void DFS(int v, int goal, List<int[]> path)
    {
        //DFS 구현 부분
        visit[v] = true;
        stack.Push(v); //스택에 값을 넣어준다.

        if (v == goal)
        { //목표노드에 왔으면

            //// 스택값 출력
            int count = stack.Count; //스택의 크기를 받을 변수
            int[] arr = stack.ToArray();

            if (arr.Length>=7 && arr.Length<=9)
            {
                path.Add(arr);
            }
            //// 스택값 출력

            stack.Pop(); //DFS에서 빠져나올땐  pop을 합니다.
            return;
        }


        for (int i = 0; i < 20; i++)
        {
            if (maps[v, i] == 1 && !visit[i])
            {
                //노드가 이어져있고 방문을 하지 않았으면
                DFS(i, goal, path);
                visit[i] = false; //DFS에서 빠져나오면 해당노드는 방문하지 않은 것으로 한다.
            }
        }

        stack.Pop(); //DFS에서 빠져나올땐 pop을 합니다.

    }

}

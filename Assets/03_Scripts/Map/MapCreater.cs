using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapCreater : MonoBehaviour
{

    public GameObject start;
    public GameObject end;
    public GameObject cube;

    public List<GameObject> TBLR;
    public GroundList groundList;

    int x1;
    int x2;

    int y1;
    int y2;

    public int branchMaxRoom = 4;
    public int startNode;
    public int endNode;

    List<int[]> path = new List<int[]>();
    //방 생성 유무 판단 메트릭스
    int[,] matrix = { { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1 } };
    //생성된 방의 열린문 판단 메트릭스
    int[,] doorMatrix = { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
    //일차적으로 생성된 방들을 큰 메트릭스로 이동시키기 위한 실제로 맵생성에 사용되는 메트릭스
    int[,] realMatrix = new int[15, 15];
    int pathSize = 0;
    int min = 5;
    //생성된 방에서 열린 문이 있다면 큐에 저장하여 방을 순차적으로 생성
    Queue<int[]> openRoom = new Queue<int[]>();
    //4*5의 경로를 구하기위한 그래프
    int[,] ta = new int[20, 20];
    int maxRoom = 20;
    int roomCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        RoomManager roomManager = RoomManager.instance;
        groundList = transform.GetChild(0).GetComponent<GroundList>();
        //4*5의 경로를 구하기위해 그래프를 메트릭스로 구현
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

        //start와 end를 랜덤한 위치로 배치하기위한 random
        x1 = Random.Range(0, 4);
        x2 = Random.Range(0, 4);
        y1 = Random.Range(0, 3);
        y2 = Random.Range(0, 3);

        //start와 end가 우연히 같다면 다시 랜덤돌리는게아니라 그냥 end의 y축을 더하거나 뺀다
        if (x1 == x2 && y1 == y2)
        {
            if (y2 != 4)
                y2++;
            else
                y2--;
        }

        //방생성 유무 판단 메트릭스에 start와 end를 지정
        matrix[3 - y1, x1] = 1;
        matrix[3 - y2, x2] = 2;

        //게임내에 start와 end의 위치를 수정
        start.transform.position = new Vector3(x1, y1);
        end.transform.position = new Vector3(x2, y2);

        //4*5에서의 길을 찾기위해 dfs알고리즘에 해당 그래프 삽입
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
        //start에서 end까지의 경로 탐색
        d.DFS(startNode, endNode, path);
        int ran = Random.Range(0, path.Count);

        //경로에 맞춰 방을 생성하며 각 방의 문을 설정
        roomManager.waveRooms = new Vector2Int[path[ran].Length];
        for (int j = 0; j < path[ran].Length - 1; j++)
        {
            Vector2Int v = new Vector2Int(path[ran][j] % 5, path[ran][j] / 5);
            matrix[3 - v.y, v.x] = 0;

            switch (path[ran][j + 1] - path[ran][j])
            {
                case 1:
                    doorMatrix[3 - v.y, v.x] |= 1;
                    doorMatrix[3 - v.y, v.x + 1] |= 2;
                    break;
                case -1:
                    doorMatrix[3 - v.y, v.x] |= 2;
                    doorMatrix[3 - v.y, v.x - 1] |= 1;
                    break;
                case 5:
                    doorMatrix[3 - v.y, v.x] |= 8;
                    doorMatrix[3 - v.y - 1, v.x] |= 4;
                    break;
                case -5:
                    doorMatrix[3 - v.y, v.x] |= 4;
                    doorMatrix[3 - v.y + 1, v.x] |= 8;
                    break;
                default:
                    Debug.Log("뭔가 잘못됨 ㅅㄱ");
                    break;
            }
            v = new Vector2Int((v.x + 5), v.y + 6);
            roomManager.waveRooms[j] = new Vector2Int(v.x,v.y);
        }

        //문 여느라 시작 끝위치가 지워져서 다시 지정
        matrix[3 - y1, x1] = 1;
        matrix[3 - y2, x2] = 2;
        for (int i = 0; i < 4; i++)
        {
            string str = "";
            for (int j = 0; j < 5; j++)
            {
                str += doorMatrix[i, j].ToString() + " ";
            }
            Debug.Log(str);
        }

        //15*15 메트릭스로 이동
        MatrixMoveToReal();

        roomCount += path[ran].Length;

        while (openRoom.Count > 0 && roomCount < maxRoom)
        {
            int[] pos = openRoom.Dequeue();
            OpenTheDoor(pos[0], pos[1]);
        }

        roomManager.rooms = new RoomInfo[15][];

        //방생성
        for (int i = 0; i < 15; i++)
        {
            string str = "";
            for (int j = 0; j < 15; j++)
            {
                str += realMatrix[i, j].ToString() + " ";
                roomManager.rooms[i] = new RoomInfo[15];

                //방생성과 동시에 바닥 생성하고 바닥을 그 방의 자식오브젝트로 둔다.
                if (realMatrix[i, j] != 0)
                {
                    Vector2 v = new Vector2(j * 30, (14 - i) * 20);
                    GameObject rm = Instantiate(TBLR[realMatrix[i, j] - 1], v, Quaternion.identity);


                    if (realMatrix[i, j] == 1)
                    {
                        int random = Random.Range(0, groundList.endGround_right.Length);
                        GameObject g = Instantiate(groundList.endGround_right[random], v, Quaternion.identity);
                        g.transform.parent = rm.transform;
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        continue;
                    }
                    else if (realMatrix[i, j] == 2)
                    {
                        int random = Random.Range(0, groundList.endGround_left.Length);
                        GameObject g = Instantiate(groundList.endGround_left[random], v, Quaternion.identity);
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        g.transform.parent = rm.transform;
                        continue;
                    }
                    else if (realMatrix[i, j] == 4)
                    {
                        int random = Random.Range(0, groundList.endGround_down.Length);
                        GameObject g = Instantiate(groundList.endGround_down[random], v, Quaternion.identity);
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        g.transform.parent = rm.transform;
                        continue;
                    }
                    else if (realMatrix[i, j] == 8)
                    {
                        int random = Random.Range(0, groundList.endGround_up.Length);
                        GameObject g = Instantiate(groundList.endGround_up[random], v, Quaternion.identity);
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        g.transform.parent = rm.transform;
                        continue;
                    }
                    else if ((realMatrix[i, j] & 1) == 1)
                    {
                        int random = Random.Range(0, groundList.connectable_right.Length);
                        GameObject g = Instantiate(groundList.connectable_right[random], v, Quaternion.identity);
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        g.transform.parent = rm.transform;
                        continue;
                    }
                    else if ((realMatrix[i, j] & 2) == 2)
                    {
                        int random = Random.Range(0, groundList.connectable_left.Length);
                        GameObject g = Instantiate(groundList.connectable_left[random], v, Quaternion.identity);
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        g.transform.parent = rm.transform;
                        continue;
                    }
                    else if ((realMatrix[i, j] & 4) == 4)
                    {
                        int random = Random.Range(0, groundList.connectable_down.Length);
                        GameObject g = Instantiate(groundList.connectable_down[random], v, Quaternion.identity);
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        g.transform.parent = rm.transform;
                        continue;
                    }
                    else if ((realMatrix[i, j] & 8) == 8)
                    {
                        int random = Random.Range(0, groundList.connectable_up.Length);
                        GameObject g = Instantiate(groundList.connectable_up[random], v, Quaternion.identity);
                        roomManager.rooms[i][j] = g.GetComponent<RoomInfo>();
                        g.transform.parent = rm.transform;
                        continue;
                    }

                }
            }
            Debug.Log(str);
        }
        Vector2 s = new Vector2((x1 + 5) * 30, (y1 + 6) * 20);
        start.transform.position = s;
        Vector2 e = new Vector2((x2 + 5) * 30, (y2 + 6) * 20);
        end.transform.position = e;

        while (openRoom.Count > 0)
        {
            int[] a = openRoom.Dequeue();
            Debug.Log(a[0].ToString() + " " + a[1].ToString());
        }

    }

    //주변을 탐색하고 이웃한 방이 없는 부분에 확률적으로 새로운 방 생성
    void OpenTheDoor(int a, int b)
    {
        if (b < 13 && realMatrix[a, b + 1] == 0)
        {
            int ran = Random.Range(0, 2);
            if (ran > 0)
                ran = 1;
            realMatrix[a, b] |= ran;
            ran = ran << 1;
            realMatrix[a, b + 1] |= ran;
            if (realMatrix[a, b + 1] != 0)
            {
                int[] enq = { a, b + 1 };
                openRoom.Enqueue(enq);
                roomCount++;
            }
        }
        if (b > 0 && realMatrix[a, b - 1] == 0)
        {
            int ran = Random.Range(0, 2);
            if (ran > 0)
                ran = 1;
            ran = ran << 1;
            realMatrix[a, b] |= ran;
            ran = ran >> 1;
            realMatrix[a, b - 1] |= ran;
            if (realMatrix[a, b - 1] != 0)
            {
                int[] enq = { a, b - 1 };
                openRoom.Enqueue(enq);
                roomCount++;
            }
        }
        if (a > 0 && realMatrix[a - 1, b] == 0)
        {
            int ran = Random.Range(0, 2);
            if (ran > 0)
                ran = 1;
            ran = ran << 3;
            realMatrix[a, b] |= ran;
            ran = ran >> 1;
            realMatrix[a - 1, b] |= ran;
            if (realMatrix[a - 1, b] != 0)
            {
                int[] enq = { a - 1, b };
                openRoom.Enqueue(enq);
                roomCount++;
            }
        }
        if (a < 13 && realMatrix[a + 1, b] == 0)
        {
            int ran = Random.Range(0, 2);
            if (ran > 0)
                ran = 1;
            ran = ran << 2;
            realMatrix[a, b] |= ran;
            ran = ran << 1;
            realMatrix[a + 1, b] |= ran;
            if (realMatrix[a + 1, b] != 0)
            {
                int[] enq = { a + 1, b };
                openRoom.Enqueue(enq);
                roomCount++;
            }
        }
    }

    //path만 가지고있는 맵에서 다양한 크기의 맵을 생성하기 위해 더 넓은 메트릭스로 이동
    void MatrixMoveToReal()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
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

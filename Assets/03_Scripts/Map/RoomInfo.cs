using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomInfo : MonoBehaviour
{
    public class CellInfo
    {
        private bool isCellAvailable = false;    // 해당 칸의 사용가능여부
        private UnitInfo unit;                  // 해당 칸에 배치된 유닛
        public Vector3 position;               // 해당 칸의 위치

        public void SetCellAvailable(bool isit) { isCellAvailable = isit; }
        public bool GetCellAvailable() { return isCellAvailable; }

        public CellInfo(int x, int y, Vector3 roomPos)
        {
            position = roomPos;
            position.x = position.x - RoomInfo.roomRow / 2 + 0.5f + x;
            position.y = position.y - RoomInfo.roomCol / 2 + 0.5f + y;
        }
    }

    public bool up, down, left, right;
    public GemGroup[] gemGroups;
    public List<Transform>  upToDown, upToLeft, upToRight, downToLeft, downToRight, leftToRight;    // 각 방향별 웨이포인트
    private Collider2D groundCol;       // 바닥 콜라이더
    private ColliderChecker colChecker;   // 칸 정보 입력용 콜라이더 체커

    public CellInfo[][] cellInfos;      // 해당 방의 칸 정보

    private HallInfo hallInfo;          // 해당 방에 연결된 통로 정보
    private GemGroup selectedGemGroup;  // 랜덤으로 선택된 젬 그룹
    [HideInInspector]
    public List<int> connectedGate;    // 연결된 통로 방향들/ 상 = 0, 하 = 1, 좌 = 2, 우 = 3    
    [HideInInspector]
    public bool isVisited = false;      // 해당 방 방문 여부

    [HideInInspector]
    public RoomIcon roomIcon;           // 해당 방과 링크된 룸 아이콘

    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;
    public static int UNKNOWN = 0, NORMAL = 1, WAVE = 2, CORE = 3, ENEMY = 4;

    public static int roomRow = 20, roomCol = 12;  // 한 방의 가로세로 칸 수

    private void Awake()
    {
        groundCol = GetComponent<TilemapCollider2D>();
        groundCol.isTrigger = true;
        colChecker = GetComponentInChildren<ColliderChecker>();
    }

    // Ground 배치 후 룸 초기화
    public void RoomInit()
    {
        // 칸 정보 배열 초기화
        cellInfos = new CellInfo[roomRow][];
        for (int i = 0; i < roomRow; ++i)
        {
            cellInfos[i] = new CellInfo[roomCol];
            for (int j = 0; j < roomCol; ++j)
            {
                cellInfos[i][j] = new CellInfo(i, j, transform.position);
            }
        }        

        // 해당 방과 연결된 통로 정보 가져오기
        hallInfo = transform.GetComponentInParent<HallInfo>();
        // 통로 연결 정보 입력
        connectedGate = new List<int>();
        for (int i = 0; i < hallInfo.gates.Length; ++i) if (hallInfo.gates[i] != null) connectedGate.Add(i);

        // 칸 정보 세팅 코루틴 활성화
        StartCoroutine(SetCellInfos());

        // 모든 통로 닫기
        CloseEveryGate();

        // 해당 방의 젬그룹 중 랜덤으로 하나 활성화
        if (gemGroups.Length > 0)
        {
            int rand = Random.Range(0, gemGroups.Length);
            selectedGemGroup = gemGroups[rand];
            selectedGemGroup.gameObject.SetActive(true);
        }
    }

    public IEnumerator SetCellInfos()
    {
        colChecker.gameObject.SetActive(true);
        colChecker.groundCol = groundCol;

        //int count = 0;

        for (int i = 0; i < roomRow; ++i)
        {
            for(int j = 0; j < roomCol; ++j)
            {
                colChecker.transform.Translate(-colChecker.transform.position);
                colChecker.transform.Translate(cellInfos[i][j].position);

                yield return new WaitForFixedUpdate();

                if (colChecker.isGround)
                {
                    //count += 1;
                    //Debug.Log("셀 활성화");
                    cellInfos[i][j].SetCellAvailable(true);
                }
            }
        }

        //Debug.Log(cellInfos[0][0].position.ToString());

        //Debug.Log("순회 끝, 활성화된 셀 = " + count);
        colChecker.gameObject.SetActive(false);

        // 문 근처 칸 비활성화
        for (int i = 0; i < hallInfo.gates.Length; ++i)
        {
            if (hallInfo.gates[i] != null)
            {
                if(i == 0)
                {
                    cellInfos[8][roomCol - 1].SetCellAvailable(false);
                    cellInfos[9][roomCol - 1].SetCellAvailable(false);
                    cellInfos[10][roomCol - 1].SetCellAvailable(false);
                    cellInfos[11][roomCol - 1].SetCellAvailable(false);
                }
                else if(i == 1)
                {
                    cellInfos[8][0].SetCellAvailable(false);
                    cellInfos[9][0].SetCellAvailable(false);
                    cellInfos[10][0].SetCellAvailable(false);
                    cellInfos[11][0].SetCellAvailable(false);
                }
                else if(i == 2)
                {
                    cellInfos[0][4].SetCellAvailable(false);
                    cellInfos[0][5].SetCellAvailable(false);
                    cellInfos[0][6].SetCellAvailable(false);
                    cellInfos[0][7].SetCellAvailable(false);
                }
                else if(i == 3)
                {
                    cellInfos[roomRow - 1][4].SetCellAvailable(false);
                    cellInfos[roomRow - 1][5].SetCellAvailable(false);
                    cellInfos[roomRow - 1][6].SetCellAvailable(false);
                    cellInfos[roomRow - 1][7].SetCellAvailable(false);
                }
            }
        }
    }

    // 입구와 출구 방향에 따른 웨이브 경로 포인트 반환/ 상 = 0, 하 = 1, 좌 = 2, 우 = 3
    public List<Transform> GetWayPoints(int entry, int exit)
    {
        if      (entry == 0 && exit == 1) return upToDown;                  // 상 -> 하
        else if (entry == 0 && exit == 2) return upToLeft;                  // 상 -> 좌
        else if (entry == 0 && exit == 3) return upToRight;                 // 상 -> 우
        else if (entry == 1 && exit == 0) return ReverseList(upToDown);     // 하 -> 상
        else if (entry == 1 && exit == 2) return downToLeft;                // 하 -> 좌
        else if (entry == 1 && exit == 3) return downToRight;               // 하 -> 우
        else if (entry == 2 && exit == 0) return ReverseList(upToLeft);     // 좌 -> 상
        else if (entry == 2 && exit == 1) return ReverseList(downToLeft);   // 좌 -> 하
        else if (entry == 2 && exit == 3) return leftToRight;               // 좌 -> 우
        else if (entry == 3 && exit == 0) return ReverseList(upToRight);    // 우 -> 상
        else if (entry == 3 && exit == 1) return ReverseList(downToRight);  // 우 -> 하
        else if (entry == 3 && exit == 2) return ReverseList(leftToRight);  // 우 -> 좌

        return null;
    }

    // 리스트 뒤집기
    private List<Transform> ReverseList(List<Transform> list)
    {
        List<Transform> reverseList = new List<Transform>();
        for (int i = 0; i < list.Count; ++i) reverseList.Add(list[i]);
        reverseList.Reverse();

        return reverseList;
    }

    // 모든 문 열기
    public void OpenEveryGate()
    {
        for (int i = 0; i < connectedGate.Count; ++i) hallInfo.OpenGate(connectedGate[i]);
    }

    // 모든 문 닫기
    public void CloseEveryGate()
    {
        for (int i = 0; i < connectedGate.Count; ++i) hallInfo.CloseGate(connectedGate[i]);
    }

    // 특정 방향 문 열기
    public void OpenGate(int dirrection)
    {
        // 문이 없거나 이미 열려있으면 패스
        if (hallInfo.gates[dirrection] == null) return;
        if (hallInfo.gates[dirrection].isOpen) return;
        hallInfo.OpenGate(dirrection);
    }

    // 특정 방향 문 닫기
    public void CloseGate(int dirrection)
    {
        // 문이 없거나 이미 닫혀있으면 패스
        if (hallInfo.gates[dirrection] == null) return;
        if (!hallInfo.gates[dirrection].isOpen) return;
        hallInfo.CloseGate(dirrection);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomManager : MonoBehaviour
{
    /*  rooms와 waveRooms에 정보를 입력한 후 MapInit을 실행하면 웨이브 경로의 문이 열리고 
        wayPoints 리스트에 웨이브 경로 포인트들이 저장됩니다
        MapInit, GetWayPoints, OpenWaveGates 함수 이외의 것은 신경안써도 됩니다    */

    // 미니맵 관련 변수


    public RoomIcon roomIcon;           // 룸 아이콘 프리팹
    public RectTransform miniMap;       // 아이콘들 부모 오브젝트
    public RectTransform currentIcon;   // 현재 방 표시 아이콘
    public GameObject mainCamera;       // 카메라    

    // 입력받는 정보들
    public RoomInfo[][] rooms;          // 배치된 방 정보들
    public RoomIcon[][] roomIcons;      // 배치된 정보에 맞는 룸 아이콘들
    public Vector2Int[] waveRooms;      // 웨이브 경로인 방 위치들 (적 본진 -> 아군 본진)

    [HideInInspector]
    public List<Transform> wayPoints;       // 웨이브 경로 포인트 리스트
    private Vector2Int[] waveDirrection;    // 각 방의 웨이브 경로 방향들 (입구, 출구)
    [HideInInspector]
    public Vector2Int currentRoom;         // 현재 플레이어가 있는 방 인덱스

    // 미니맵 관련 수치
    private static int iconX = 50, iconY = 38;  // 아이콘 배치 간격
    private Vector3 minimapPosition;            // 미니맵 기본위치(웨이브 경로가 보이는)
    private float cameraOffsetY = -0.6f;        // 카메라 위치조정 오프셋

    // 편의용 상수
    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;
    public static int UNKNOWN = 0, NORMAL = 1, WAVE = 2, CORE = 3, ENEMY = 4;

    public static RoomManager instance;

    private void Awake()
    {
        RoomManager.instance = this;
    }

    // 배치정보를 다 받아온 후 실행. 각 방들 초기화, 최종 웨이포인트 설정과 해당 경로의 문 열기, 미니맵 아이콘 표시
    public void MapInit()
    {
        // 디버깅
        for(int i = 0; i < rooms.Length; ++i)
        {
            for(int j = 0; i < rooms[i].Length; ++j)
            {
                if (rooms[i][j] == null) Debug.Log("(" + i + ", " + j + ") 는 비어 있습니다");
                else
                {
                    Debug.Log("(" + i + ", " + j + ") 방의 이름은 + " + rooms[i][j].name);
                }
            }
        }

        roomIcons = new RoomIcon[rooms.Length][];
        for (int i = 0; i < rooms.Length; ++i) roomIcons[i] = new RoomIcon[rooms[i].Length];

        // 아이콘 위치
        Vector3 iconPos = Vector3.zero;

        for(int i = 0; i < rooms.Length; ++i)
        {
            iconPos.y = 0;
            for(int j = 0; j < rooms[i].Length; ++j)
            {
                if (rooms[i][j] != null)
                {
                    // 각 방들 초기화
                    rooms[i][j].RoomInit();

                    // 각 방별 아이콘 생성 및 연결
                    roomIcons[i][j] = Instantiate(roomIcon, miniMap);
                    rooms[i][j].roomIcon = roomIcons[i][j];
                    roomIcons[i][j].roomInfo = rooms[i][j];
                    roomIcons[i][j].rect.Translate(iconPos);
                    roomIcons[i][j].gameObject.SetActive(false);
                }
                iconPos.y += iconY;
            }
            iconPos.x += iconX;
        }
        // 현재방 아이콘 순서 밀기
        currentIcon.SetAsLastSibling();

        // 플레이어가 속한 방(코어방) 세팅
        currentRoom = waveRooms[waveRooms.Length-1];
        rooms[currentRoom.x][currentRoom.y].isVisited = true;

        // 미니맵 위치 조정
        minimapPosition = MinimapPositionInit();

        // 카메라와 아이콘 이동
        MoveCamera(currentRoom);
        currentIcon.DOMove(roomIcons[currentRoom.x][currentRoom.y].transform.position, 0.5f);

        // 웨이포인트 리스트 생성
        wayPoints = GetWayPoints();
        // 웨이브 경로 열기
        OpenWaveGates();
    }

    public void VisitRoom(Vector2Int index)
    {
        currentRoom = index;
        int x = index.x, y = index.y;

        // 카메라와 아이콘 이동
        MoveCamera(index);
        currentIcon.DOMove(roomIcons[x][y].transform.position, 0.5f);        

        // 이미 방문한 경우 리턴
        if (rooms[x][y].isVisited) return;

        rooms[x][y].isVisited = true;

        int waveRoomIndex = isWaveRoom(index);

        // 웨이브 룸의 경우 
        if (waveRoomIndex != -1)
        {
            // 방 아이콘 설정
            roomIcons[x][y].SetRoomIcon(WAVE);

            // 통로 아이콘 설정
            for(int i = 0; i < rooms[x][y].connectedGate.Count; ++i)
            {
                // 해당 룸의 연결된 통로방향
                int dirrection = rooms[x][y].connectedGate[i];

                // 웨이브 경로는 웨이브로 표시
                if (dirrection == waveDirrection[waveRoomIndex].x || dirrection == waveDirrection[waveRoomIndex].y)
                {
                    SetLinkedHall(index, WAVE, dirrection);
                }
                // 웨이브 경로가 아니고 아직 안 밝혀진 경우 UNKNOWN으로 표시
                else
                {
                    if(roomIcons[x][y].hallState[dirrection] == -1) SetLinkedHall(index, UNKNOWN, dirrection);
                }
            }
        }
        // 웨이브 룸이 아닌 경우
        else if (waveRoomIndex == -1)
        {
            // 방 아이콘 설정
            roomIcons[x][y].SetRoomIcon(NORMAL);

            // 통로 아이콘 설정
            for (int i = 0; i < rooms[x][y].connectedGate.Count; ++i)
            {
                // 해당 룸의 연결된 통로방향
                int dirrection = rooms[x][y].connectedGate[i];
                // 아직 안 밝혀진 경우 UNKNOWN으로 표시
                if (roomIcons[x][y].hallState[dirrection] == -1) SetLinkedHall(index, UNKNOWN, dirrection);
            }
        }
    }

    // 현재 방 클리어
    public void RoomClear()
    {
        RoomClear(currentRoom);
    }

    // 해당 방 클리어(해당 방의 문과 연결된 방의 문 열기) - 필드몹 모두 잡으면 실행
    public void RoomClear(Vector2Int index)
    {
        int x = index.x, y = index.y;

        RoomInfo room = rooms[x][y];
        if (room == null) return;

        // 모든 문 열기
        room.OpenEveryGate();

        for (int i = 0; i < room.connectedGate.Count; ++i)
        {
            // 해당 룸의 연결된 통로방향
            int dirrection = room.connectedGate[i];

            // 연결된 방의 해당 문도 열기
            GetNearRoom(index, dirrection).OpenGate(InverseDirrection(dirrection));
        }

        // 미니맵 표시
        for (int i = 0; i < room.connectedGate.Count; ++i)
        {
            // 해당 룸의 연결된 통로방향
            int dirrection = room.connectedGate[i];
            // 웨이브 경로가 아닌 경우 NORMAL로 표시
            if (roomIcons[x][y].hallState[dirrection] != WAVE) SetLinkedHall(index, NORMAL, dirrection);

            // 방문하지 않은 방의 존재 표시
            if (!GetNearRoom(index, dirrection).isVisited)
            {
                GetNearIcon(index, dirrection).gameObject.SetActive(true);
                GetNearIcon(index, dirrection).SetRoomIcon(UNKNOWN);
            }
        }
    }

    // 최종 웨이브 경로 포인트 리스트 반환
    private List<Transform> GetWayPoints()
    {
        GetWaveDirrection();

        List<Transform> points = new List<Transform>();
        
        for(int i = 0; i < waveRooms.Length; ++i)
        {
            if (rooms[waveRooms[i].x][waveRooms[i].y] == null) Debug.Log("null");
            points.AddRange(rooms[waveRooms[i].x][waveRooms[i].y].GetWayPoints(waveDirrection[i].x, waveDirrection[i].y));
        }

        return points;
    }

    // 웨이브 경로의 문 열기
    private void OpenWaveGates()
    {
        for (int i = 0; i < waveRooms.Length; ++i)
        {
            int x = waveRooms[i].x;
            int y = waveRooms[i].y;

            // 문 열기
            rooms[x][y].OpenGate(waveDirrection[i].x);
            rooms[x][y].OpenGate(waveDirrection[i].y);

            // 웨이브 경로 통로 아이콘 표시
            if (i == 0) roomIcons[x][y].SetHallState(UNKNOWN, waveDirrection[i].y);
            else if (i == waveRooms.Length - 1) roomIcons[x][y].SetHallState(WAVE, waveDirrection[i].x);
            else if (i == waveRooms.Length - 2)
            {
                roomIcons[x][y].SetHallState(UNKNOWN, waveDirrection[i].x);
                roomIcons[x][y].SetHallState(WAVE, waveDirrection[i].y);
            }
            else
            {
                roomIcons[x][y].SetHallState(UNKNOWN, waveDirrection[i].x);
                roomIcons[x][y].SetHallState(UNKNOWN, waveDirrection[i].y);
            }
        }
    }

    // 정보를 토대로 각 방의 웨이브 방향 정보 계산
    private void GetWaveDirrection()
    {
        waveDirrection = new Vector2Int[waveRooms.Length];

        int x = waveRooms[0].x;
        int y = waveRooms[0].y;

        // 시작 방의 경로방향은 따로 계산
        waveDirrection[0].y = GetDirrection(waveRooms[0], waveRooms[1]);
        waveDirrection[0].x = InverseDirrection(waveDirrection[0].y);
        // 시작 방 아이콘 세팅
        roomIcons[x][y].gameObject.SetActive(true);
        roomIcons[x][y].SetRoomIcon(ENEMY);

        // 중간 방의 경로들 계산. 입구 방향은 이전 방의 출구방향과 같음, 출구방향은 다음 방의 위치로 계산
        for (int i = 1; i < waveRooms.Length - 1; ++i)
        {
            waveDirrection[i].x = InverseDirrection(waveDirrection[i - 1].y);
            waveDirrection[i].y = GetDirrection(waveRooms[i], waveRooms[i + 1]);

            // 중간 방 아이콘 세팅
            x = waveRooms[i].x;
            y = waveRooms[i].y;
            rooms[x][y].roomIcon.gameObject.SetActive(true);
            rooms[x][y].roomIcon.SetRoomIcon(UNKNOWN);
        }

        // 끝 방의 경로방향도 따로 계산
        int last = waveRooms.Length - 1;
        waveDirrection[last].x = InverseDirrection(waveDirrection[last - 1].y);
        waveDirrection[last].y = InverseDirrection(waveDirrection[last].x);

        // 끝 방 아이콘 세팅
        x = waveRooms[last].x;
        y = waveRooms[last].y;
        rooms[x][y].roomIcon.gameObject.SetActive(true);
        rooms[x][y].roomIcon.SetRoomIcon(CORE);
    }

    // 입력한 방향의 반대방향 뱉기
    private int InverseDirrection(int dir)
    {
        int inverse;
        inverse = dir == 0 || dir == 2 ? dir + 1 : dir - 1;
        return inverse;
    }

    // 현재 좌표와 다음 좌표 계산해서 방향 도출
    private int GetDirrection(Vector2 pos, Vector2 nextPos)
    {
        Vector2 dir = nextPos - pos;

        if (dir.x == 1) return RIGHT;
        if (dir.x == -1) return LEFT;
        if (dir.y == 1) return UP;
        if (dir.y == -1) return DOWN;

        return -1;
    }

    // 해당 방이 웨이브 경로인지 파악 (아군, 적 본진 제외)
    private int isWaveRoom(Vector2Int index)
    {
        for (int i = 1; i < waveRooms.Length - 1; ++i) if (waveRooms[i] == index) return i;

        return -1;
    }

    // 미니맵의 통로 세팅 시 해당 통로와 연결된 다른 방의 통로도 같이 설정
    private void SetLinkedHall(Vector2Int index, int state, int dirrection)
    {
        int x = index.x, y = index.y;
        roomIcons[x][y].SetHallState(state, dirrection);

        GetNearIcon(index, dirrection).SetHallState(state, InverseDirrection(dirrection));
    }

    // 해당 좌표 기준 입력 방향의 방 리턴
    private RoomInfo GetNearRoom(Vector2Int index, int dirrection)
    {
        int x = index.x, y = index.y;

        if (dirrection == UP)           return rooms[x][y + 1];
        else if (dirrection == DOWN)    return rooms[x][y - 1];
        else if (dirrection == LEFT)    return rooms[x - 1][y];
        else if (dirrection == RIGHT)   return rooms[x + 1][y];

        return null;
    }

    // 해당 좌표 기준 입력 방향의 방 아이콘 리턴
    private RoomIcon GetNearIcon(Vector2Int index, int dirrection)
    {
        int x = index.x, y = index.y;

        if (dirrection == UP)           return roomIcons[x][y + 1];
        else if (dirrection == DOWN)    return roomIcons[x][y - 1];
        else if (dirrection == LEFT)    return roomIcons[x - 1][y];
        else if (dirrection == RIGHT)   return roomIcons[x + 1][y];

        return null;
    }

    // 해당 방으로 카메라 이동
    private void MoveCamera(Vector2Int index)
    {
        Vector3 target = rooms[index.x][index.y].transform.position;
        target.y += cameraOffsetY;
        target.z -= 10;
        mainCamera.transform.DOMove(target, 0.5f);
    }

    // 웨이브 경로가 보이도록 미니맵 옮기기
    private Vector3 MinimapPositionInit()
    {
        int x = 0, y = 0;
        for(int i = 0; i < waveRooms.Length; ++i)
        {
            x = waveRooms[i].x > x ? waveRooms[i].x : x;
            y = waveRooms[i].y > y ? waveRooms[i].y : y;
        }
        x -= 2;
        y -= 2;
        // 경로가 맵에 한눈에 보이도록 옮기기
        miniMap.Translate(iconX * -(0.5f + x), iconY * -y, 0);
                
        int smallX = 100, bigX = 0, smallY = 100, bigY = 0, lengthX, lengthY;

        // 웨이브 총 경로의 가로세로 길이 재기
        for (int i = 0; i < waveRooms.Length; ++i)
        {
            smallX = waveRooms[i].x < smallX ? waveRooms[i].x : smallX;
            smallY = waveRooms[i].y < smallY ? waveRooms[i].y : smallY;
            bigX = waveRooms[i].x >= bigX ? waveRooms[i].x : bigX;
            bigY = waveRooms[i].y >= bigY ? waveRooms[i].y : bigY;
        }

        lengthX = bigX - smallX + 1;
        lengthY = bigY = smallY + 1;

        int moveX = Mathf.RoundToInt((4 - lengthX) / 2);
        int moveY = Mathf.RoundToInt((4 - lengthY) / 2);

        // 여유가 있다면 중간으로 옮기기
        miniMap.Translate(iconX * -moveX, iconY * -moveY, 0);

        return miniMap.localPosition;
    }
}

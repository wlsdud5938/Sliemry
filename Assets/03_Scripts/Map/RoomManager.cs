using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    /*  rooms와 waveRooms에 정보를 입력한 후 MapInit을 실행하면 웨이브 경로의 문이 열리고 
        wayPoints 리스트에 웨이브 경로 포인트들이 저장됩니다
        MapInit, GetWayPoints, OpenWaveGates 함수 이외의 것은 신경안써도 됩니다    */

    public RoomInfo[][] rooms;          // 배치된 방 정보들
    public Vector2Int[] waveRooms;         // 웨이브 경로인 방 위치들 (적 본진 -> 아군 본진)

    [HideInInspector]
    public List<Transform> wayPoints;   // 웨이브 경로 포인트 리스트
    private Vector2Int[] waveDirrection;    // 각 방의 웨이브 경로 방향들 (입구, 출구)

    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;

    // 배치정보를 다 받아온 후 실행. 각 방들 초기화, 최종 웨이포인트 설정과 해당 경로의 문 열기
    public void MapInit()
    {
        for(int i = 0; i < rooms.Length; ++i)
        {
            for(int j = 0; j < rooms[i].Length; ++j)
            {
                if (rooms[i][j] != null) rooms[i][j].RoomInit();
            }
        }

        wayPoints = GetWayPoints();
        OpenWaveGates();
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
            rooms[waveRooms[i].x][waveRooms[i].y].OpenGate(waveDirrection[i].x);
            rooms[waveRooms[i].x][waveRooms[i].y].OpenGate(waveDirrection[i].y);
        }
    }

    // 정보를 토대로 각 방의 웨이브 방향 정보 계산
    private void GetWaveDirrection()
    {
        waveDirrection = new Vector2Int[waveRooms.Length];

        // 시작 방의 경로방향은 따로 계산
        waveDirrection[0].y = GetDirrection(waveRooms[0], waveRooms[1]);
        waveDirrection[0].x = InverseDirrection(waveDirrection[0].y);

        // 중간 방의 경로들 계산. 입구 방향은 이전 방의 출구방향과 같음, 출구방향은 다음 방의 위치로 계산
        for (int i = 1; i < waveRooms.Length - 1; ++i)
        {
            waveDirrection[i].x = InverseDirrection(waveDirrection[i - 1].y);
            waveDirrection[i].y = GetDirrection(waveRooms[i], waveRooms[i + 1]);
        }

        // 끝 방의 경로방향도 따로 계산
        int last = waveRooms.Length - 1;
        waveDirrection[last].x = InverseDirrection(waveDirrection[last - 1].y);
        waveDirrection[last].y = InverseDirrection(waveDirrection[last].x);
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
}

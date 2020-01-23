using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomIcon : MonoBehaviour
{
    public RectTransform rect;
    public Image roomBigPart;                               // 통이미지 전용(코어룸, 적본진룸) 통째 이미지
    public Image[] roomParts, hallParts, openParts;         // 아이콘의 이미지 파트들
    public Sprite[] normalRoom, waveRoom, unknownRoom;      // 각 타입의 방 스프라이트
    public Sprite[] normal_open, wave_open, unknown_open;   // 각 타입의 방 통로와 연결된 부분 스프라이트
    public Sprite[] coreRoom, enemyRoom;                    // 본진과 적본진 상하좌우 스프라이트
    public Sprite normalHall, waveHall, unknownHall;        // 각 타입의 복도 스프라이트

    [HideInInspector]
    public int roomState;                           // 해당 방의 상태
    [HideInInspector]
    public int[] hallState = { -1, -1, -1, -1};            // 해당 방 통로의 상태

    public static int UNKNOWN = 0, NORMAL = 1, WAVE = 2, CORE = 3, ENEMY = 4;
    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;

    [HideInInspector]
    public RoomInfo roomInfo;   // 해당 아이콘에 연결된 RoomInfo

    private Sprite[] roomSprites, openSprites;  // 해당 아이콘에 적용되는 룸 스프라이트
    
    // 룸 상태 세팅
    public void SetRoomIcon(int state, List<int> connection)
    {
        roomState = state;

        if (state == UNKNOWN || state == NORMAL || state == WAVE)
        {
            SetRoomState(state);
            SetConnection(connection);
        }
        else SetCornerRoom(state, connection);
    }

    public void SetRoomIcon(int state)
    {
        SetRoomIcon(state, roomInfo.connectedGate);
    }

    // 복도 상태 세팅
    public void SetHallState(int state, List<int> dirrections)
    {
        for (int i = 0; i < dirrections.Count; ++i) SetHallState(state, dirrections[i]);
    }

    public void SetHallState(int state)
    {
        SetHallState(state, roomInfo.connectedGate);
    }

    // 복도 상태 세팅
    public void SetHallState(int state, int dirrection)
    {
        hallState[dirrection] = state;

        hallParts[dirrection].gameObject.SetActive(true);

        if (state == UNKNOWN) hallParts[dirrection].sprite = unknownHall;
        else if (state == NORMAL) hallParts[dirrection].sprite = normalHall;
        else if (state == WAVE) hallParts[dirrection].sprite = waveHall;
    }

    // 상태 설정과 그에 맞는 스프라이트 연결
    private void SetRoomState(int state)
    {
        roomState = state;
        if (state == NORMAL)
        {
            roomSprites = normalRoom;
            openSprites = normal_open;
        }
        else if (state == WAVE)
        {
            roomSprites = waveRoom;
            openSprites = wave_open;
        }
        else if (state == UNKNOWN)
        {
            roomSprites = unknownRoom;
            openSprites = unknown_open;
        }

        for (int i = 0; i < roomParts.Length; ++i) roomParts[i].sprite = roomSprites[i];
    }

    // 연결방향에 맞게 스프라이트 교체 및 활성화
    private void SetConnection(List<int> connection)
    {
        for(int i = 0; i < connection.Count; ++i)
        {
            // 해당 방향 가져와서 통로 활성화 및 열린 스프라이트로 교체
            int dir = connection[i];
            openParts[dir].sprite = openSprites[dir];
        }
    }

    // 모서리방 전용 세팅
    private void SetCornerRoom(int state, List<int> connection)
    {
        // 해당 방향의 전용 스프라이트로 교체
        for (int i = 0; i < roomParts.Length; ++i) roomParts[i].gameObject.SetActive(false);
        roomBigPart.sprite = state == CORE ? coreRoom[connection[0]] : enemyRoom[connection[0]];
    }
}

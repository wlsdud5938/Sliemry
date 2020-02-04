using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundList : MonoBehaviour
{
    // 모든 Ground 프리팹들
    public RoomInfo[] roomPrefabs;

    // 상하좌우 각 연결 가능한 Ground 프리팹들
    public GameObject[] connectable_up, connectable_down, connectable_left, connectable_right;

    // 적 및 아군 본진 전용 모서리 방들 상하좌우
    public GameObject[] endGround_up, endGround_down, endGround_left, endGround_right;
    
    public List<RoomInfo>[] grounds;

    public static int UXXX = 0, XDXX = 1, XXLX = 2, XXXR = 3, UDXX = 4, UXLX = 5, UXXR = 6, XDLX = 7, 
                      XDXR = 8, XXLR = 9, UDLX = 10, UDXR = 11, UXLR = 12, XDLR = 13, UDLR = 14;

    private void Awake()
    {
        grounds = new List<RoomInfo>[15];

        grounds[UXXX] = GetRooms(true, false, false, false);
        grounds[XDXX] = GetRooms(false, true, false, false);
        grounds[XXLX] = GetRooms(false, false, true, false);
        grounds[XXXR] = GetRooms(false, false, false, true);
        grounds[UDXX] = GetRooms(true, true, false, false);
        grounds[UXLX] = GetRooms(true, false, true, false);
        grounds[UXXR] = GetRooms(true, false, false, true);
        grounds[XDLX] = GetRooms(false, true, true, false);
        grounds[XDXR] = GetRooms(false, true, false, true);
        grounds[XXLR] = GetRooms(false, false, true, true);
        grounds[UDLX] = GetRooms(true, true, true, false);
        grounds[UDXR] = GetRooms(true, true, false, true);
        grounds[UXLR] = GetRooms(true, false, true, true);
        grounds[XDLR] = GetRooms(false, true, true, true);
        grounds[UDLR] = GetRooms(true, true, true, true);
    }

    // 입력된 연결 방향에 부합하는 방 리스트 리턴
    private List<RoomInfo> GetRooms(bool up, bool down, bool left, bool right)
    {
        List<RoomInfo> rooms = new List<RoomInfo>();        

        for(int i = 0; i < roomPrefabs.Length; ++i)
        {
            if (up && !roomPrefabs[i].up) continue;
            if (down && !roomPrefabs[i].down) continue;
            if (left && !roomPrefabs[i].left) continue;
            if (right && !roomPrefabs[i].right) continue;

            rooms.Add(roomPrefabs[i]);
        }

        /*
        Debug.Log("rooms의 count는 = " + rooms.Count);
        Debug.Log(up.ToString() + " " + down.ToString() + " " + left.ToString() + " " + right.ToString() + " 의 그라운드 결과는");
        for (int i = 0; i < rooms.Count; ++i) Debug.Log(rooms[i].name);

        Debug.Log("GetRooms 종료");
        */

        return rooms;
    }
}

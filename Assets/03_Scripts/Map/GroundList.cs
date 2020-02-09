using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundList : MonoBehaviour
{
    // 모든 Ground 프리팹들
    public GameObject[] roomPrefabs;

    // 상하좌우 각 연결 가능한 Ground 프리팹들
    public GameObject[] connectable_up, connectable_down, connectable_left, connectable_right;

    // 적 및 아군 본진 전용 모서리 방들 상하좌우
    public GameObject[] endGround_up, endGround_down, endGround_left, endGround_right;
    
    public List<GameObject>[] grounds;

    public static int UXXX = 8, XDXX = 4, XXLX = 2, XXXR = 1, UDXX = 12, UXLX = 10, UXXR = 9, XDLX = 6, 
                      XDXR = 5, XXLR = 3, UDLX = 14, UDXR = 13, UXLR = 11, XDLR = 7, UDLR = 15;

    private void Awake()
    {
        grounds = new List<GameObject>[16];

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
    private List<GameObject> GetRooms(bool up, bool down, bool left, bool right)
    {
        List<GameObject> rooms = new List<GameObject>();        

        for(int i = 0; i < roomPrefabs.Length; ++i)
        {
            if (up && !roomPrefabs[i].GetComponent<RoomInfo>().up) continue;
            if (down && !roomPrefabs[i].GetComponent<RoomInfo>().down) continue;
            if (left && !roomPrefabs[i].GetComponent<RoomInfo>().left) continue;
            if (right && !roomPrefabs[i].GetComponent<RoomInfo>().right) continue;

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

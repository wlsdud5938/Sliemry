using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    public RoomManager roomManager;
    public RoomInfo[] testRooms0, testRooms1, testRooms2, testRooms3;

    private void Start()
    {
        roomManager.rooms = new RoomInfo[4][];
        roomManager.rooms[0] = testRooms0;
        roomManager.rooms[1] = testRooms1;
        roomManager.rooms[2] = testRooms2;
        roomManager.rooms[3] = testRooms3;

        roomManager.waveRooms = new Vector2Int[6];
        roomManager.waveRooms[0] = new Vector2Int(0, 2);
        roomManager.waveRooms[1] = new Vector2Int(1, 2);
        roomManager.waveRooms[2] = new Vector2Int(1, 1);
        roomManager.waveRooms[3] = new Vector2Int(1, 0);
        roomManager.waveRooms[4] = new Vector2Int(2, 0);
        roomManager.waveRooms[5] = new Vector2Int(3, 0);
    }
}

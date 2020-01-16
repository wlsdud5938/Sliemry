using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    public RoomInfo[] testRooms;
    public List<Transform> wavePoints;

    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;

    public void RoomInit()
    {
        for (int i = 0; i < testRooms.Length; ++i) testRooms[i].RoomInit();
    }

    public void OpenWaveGates()
    {
        testRooms[0].OpenGate(LEFT);
        testRooms[0].OpenGate(RIGHT);
        testRooms[1].OpenGate(LEFT);
        testRooms[1].OpenGate(DOWN);
        testRooms[2].OpenGate(UP);
        testRooms[2].OpenGate(DOWN);
    }

    public void GetWayPoints()
    {
        wavePoints = new List<Transform>();
        wavePoints.AddRange(testRooms[0].GetWayPoints(LEFT, RIGHT));
        wavePoints.AddRange(testRooms[1].GetWayPoints(LEFT, DOWN));
        wavePoints.AddRange(testRooms[2].GetWayPoints(UP, DOWN));
    }
}

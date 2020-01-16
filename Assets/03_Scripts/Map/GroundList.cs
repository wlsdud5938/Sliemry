using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundList : MonoBehaviour
{
    // 상하좌우 각 연결 가능한 Ground 프리팹들
    public GameObject[] connectable_up, connectable_down, connectable_left, connectable_right;

    // 적 및 아군 본진 전용 모서리 방들 상하좌우
    public GameObject[] endGround_up, endGround_down, endGround_left, endGround_right;
}

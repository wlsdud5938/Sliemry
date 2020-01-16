using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallInfo : MonoBehaviour
{
    public HallGate[] gates;    // 각 통로들, 상하좌우 순서로 저장

    public void OpenGate(int dirrection)
    {
        if (gates[dirrection] != null) gates[dirrection].OpenGate();
    }

    public void CloseGate(int dirrection)
    {
        if (gates[dirrection] != null) gates[dirrection].CloseGate();
    }

}

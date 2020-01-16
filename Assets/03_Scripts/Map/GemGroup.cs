using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGroup : MonoBehaviour
{    
    public Gem[] gems;          // 해당 그룹에 속한 젬들
    [HideInInspector]
    public int unbrokenGem;     // 아직 안깨진 젬의 숫자
    
    private RoomInfo roomInfo;  // 해당 그룹이 속한 roomInfo

    private void Start()
    {
        unbrokenGem = gems.Length;
        roomInfo = transform.GetComponentInParent<RoomInfo>();
    }

    public void GemBreak()
    {
        unbrokenGem -= 1;

        // 모든 젬이 깨지면 게이트 오픈(임시 - 차후 해당 방의 모든 몹 제거로 변경)
        if (unbrokenGem == 0) roomInfo.OpenEveryGate();
    }
}

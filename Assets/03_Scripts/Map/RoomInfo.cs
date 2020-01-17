using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public GemGroup[] gemGroups;
    public List<Transform>  upToDown, upToLeft, upToRight, downToLeft, downToRight, leftToRight;    // 각 방향별 웨이포인트

    private HallInfo hallInfo;          // 해당 방에 연결된 통로 정보
    private GemGroup selectedGemGroup;  // 랜덤으로 선택된 젬 그룹
    private List<int> connectedGate;    // 연결된 통로 방향들/ 상 = 0, 하 = 1, 좌 = 2, 우 = 3    

    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;
    
    // Ground 배치 후 룸 초기화
    public void RoomInit()
    {
        // 해당 방과 연결된 통로 정보 가져오기
        hallInfo = transform.GetComponentInParent<HallInfo>();
        // 통로 연결 정보 입력
        connectedGate = new List<int>();
        for (int i = 0; i < hallInfo.gates.Length; ++i) if (hallInfo.gates[i] != null) connectedGate.Add(i);

        // 모든 통로 닫기
        CloseEveryGate();

        // 해당 방의 젬그룹 중 랜덤으로 하나 활성화
        if (gemGroups.Length > 0)
        {
            int rand = Random.Range(0, gemGroups.Length);
            selectedGemGroup = gemGroups[rand];
            selectedGemGroup.gameObject.SetActive(true);
        }
    }

    // 입구와 출구 방향에 따른 웨이브 경로 포인트 반환/ 상 = 0, 하 = 1, 좌 = 2, 우 = 3
    public List<Transform> GetWayPoints(int entry, int exit)
    {
        if      (entry == 0 && exit == 1) return upToDown;                  // 상 -> 하
        else if (entry == 0 && exit == 2) return upToLeft;                  // 상 -> 좌
        else if (entry == 0 && exit == 3) return upToRight;                 // 상 -> 우
        else if (entry == 1 && exit == 0) return ReverseList(upToDown);     // 하 -> 상
        else if (entry == 1 && exit == 2) return downToLeft;                // 하 -> 좌
        else if (entry == 1 && exit == 3) return downToRight;               // 하 -> 우
        else if (entry == 2 && exit == 0) return ReverseList(upToLeft);     // 좌 -> 상
        else if (entry == 2 && exit == 1) return ReverseList(downToLeft);   // 좌 -> 하
        else if (entry == 2 && exit == 3) return leftToRight;               // 좌 -> 우
        else if (entry == 3 && exit == 0) return ReverseList(upToRight);    // 우 -> 상
        else if (entry == 3 && exit == 1) return ReverseList(downToRight);  // 우 -> 하
        else if (entry == 3 && exit == 2) return ReverseList(leftToRight);  // 우 -> 좌

        return null;
    }

    // 리스트 뒤집기
    private List<Transform> ReverseList(List<Transform> list)
    {
        List<Transform> reverseList = new List<Transform>();
        for (int i = 0; i < list.Count; ++i) reverseList.Add(list[i]);
        reverseList.Reverse();

        return reverseList;
    }

    // 모든 문 열기
    public void OpenEveryGate()
    {
        for (int i = 0; i < connectedGate.Count; ++i) hallInfo.OpenGate(connectedGate[i]);
    }

    // 모든 문 닫기
    public void CloseEveryGate()
    {
        for (int i = 0; i < connectedGate.Count; ++i) hallInfo.CloseGate(connectedGate[i]);
    }

    // 특정 방향 문 열기
    public void OpenGate(int dirrection)
    {
        hallInfo.OpenGate(dirrection);
    }

    // 특정 방향 문 닫기
    public void CloseGate(int dirrection)
    {
        hallInfo.CloseGate(dirrection);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public GameObject[] messageObjects;         // 메세지 박스 오브젝트들 3개(순서 중요)
    public Text[] messageTexts;                 // 메세지 텍스트 3개(순서 중요)

    private int count = 0;                      // 현재 보여지는 메세지 수
    private static float time = 2.0f;           // 메세지가 떠있는 시간
    private static string empty = "";
    private IEnumerator countDownCor;

    public static MessageManager Instance;

    private void Awake()
    {
        Instance = this;
        countDownCor = Countdown();
    }

    public void ShowMessage(string message)
    {
        StopCoroutine(countDownCor);
        countDownCor = Countdown();
        StartCoroutine(countDownCor);

        if(count == 0)
        {
            messageObjects[0].SetActive(true);
            messageTexts[0].text = message;
            count += 1;
        }
        else if(count == 1)
        {
            messageObjects[1].SetActive(true);
            messageTexts[1].text = messageTexts[0].text;
            messageTexts[0].text = message;
            count += 1;
        }
        else if(count == 2)
        {
            messageObjects[2].SetActive(true);
            messageTexts[2].text = messageTexts[1].text;
            messageTexts[1].text = messageTexts[0].text;
            messageTexts[0].text = message;
            count += 1;
        }
        else if(count >= 3)
        {
            messageTexts[2].text = messageTexts[1].text;
            messageTexts[1].text = messageTexts[0].text;
            messageTexts[0].text = message;
        }
    }

    // 모든 메세지 사라지기
    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(time);
        
        for (int i = 0; i < 3; ++i) messageTexts[i].text = empty;
        for (int i = 0; i < 3; ++i) messageObjects[i].SetActive(false);
        count = 0;
    }
}

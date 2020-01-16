using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gem : MonoBehaviour
{
    private GemGroup gemGroup;
    private Animator ani;
    private static Vector3 shake = new Vector3(0.2f, 0, 0);

    private void Start()
    {
        gemGroup = transform.GetComponentInParent<GemGroup>();
        ani = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        // 마우스 우클릭으로 구슬 깨기(임시)
        if (Input.GetMouseButtonDown(1)) Break();
    }

    // 파괴될 시 해당 방의 몹 스폰
    private void Break()
    {
        ani.SetTrigger("break");
        SpawnMobs();
        gemGroup.GemBreak();
    }

    public void Shake()
    {
        transform.DOShakePosition(0.5f, shake, 10, 80.0f, false, true);
    }

    private void SpawnMobs()
    {

    }
}

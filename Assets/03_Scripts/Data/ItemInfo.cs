using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public string itemName;
    public int itemIndex = 0;
    public Sprite itemSprite;
    public bool isAttackItem;   // 공격용 아이템인지 방어용 아이템인지
    public int value = 1;

    private Rigidbody2D rig;
    private Vector3 popVector = Vector3.zero;
    private static int popDirection = 0;
    private static float popPowerFactor = 6.0f;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    public void Drop(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        // 터지는 연출
        popVector.x = Random.Range(0, 100);
        popVector.y = Random.Range(0, 100);

        if (itemIndex == 0)
        {
            if (popDirection == 0) { popVector.x *= -1; }
            else if (popDirection == 1) popVector.y *= -1;
            else if (popDirection == 2)
            {
                popVector.x *= -1;
                popVector.y *= -1;
            }
            popDirection = popDirection == 3 ? 0 : popDirection + 1;
        }
        else
        {
            if (Random.Range(0, 2) == 0) popVector.x *= -1;
            if (Random.Range(0, 2) == 0) popVector.y *= -1; 
        }

        popVector *= popPowerFactor;

        rig.AddForce(popVector);

        StartCoroutine(Disappear());
    }

    // 일정 시간 후 사라짐
    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(30.0f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.gameObject.CompareTag("Player")) //플레이어랑 충돌시
        {
            if (itemIndex == 0)
            {
                if (DataManager.Instance.userData_item.IsMoneyFull())
                {
                    MessageManager.Instance.ShowMessage(AlarmManager.moneyFull);
                    return;
                }
                DataManager.Instance.userData_item.EarnMoney(value); // 인덱스 0번이면(돈이면) 돈 추가
            }    
            else
            {
                if (DataManager.Instance.userData_item.IsItemFull())
                {
                    MessageManager.Instance.ShowMessage(AlarmManager.itemFull);
                    return;
                }
                DataManager.Instance.userData_item.EarnItem(itemName);
            }

            gameObject.SetActive(false);

            SlotManager.Instance.SetSlots();
            InvenIconManager.Instance.ItemGettingEffect(itemIndex);
        }
    }
}

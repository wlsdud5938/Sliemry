using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InvenIconManager : MonoBehaviour
{
    public Slider moneyInven;                                       // 가진 돈의 양 표시하는 슬라이더      
    public Image itemInven;                                         // 가진 아이템의 갯수 표시하는 이미지
    public Sprite[] itemInvenSprites;                               // 아이템 갯수별 이미지 스프라이트들
    public Text moneyAmount, itemAmount, moneyLimit, itemLimit;     // 숫자로 표시하는 텍스트들
    public GameObject moneyInfo, itemInfo;                          // 껐다켰다 할 오브젝트
    public Image moneyShadow, itemShadow;                           // 투명도 변화 줄 오브젝트
    
    public static InvenIconManager Instance;    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MoneyInfoOnOff(false);
        ItemInfoOnOff(false);

        // 최댓값 등록
        moneyInven.maxValue = UserItemData.moneyLimit;
        moneyLimit.text = UserItemData.moneyLimit.ToString();
        itemLimit.text = UserItemData.itemLimit.ToString();

        moneyInven.value = DataManager.Instance.userData_item.GetUserMoney();

        ShowMoney();
        ShowItem();
    }

    public void ShowMoney()
    {
        moneyInven.DOValue(DataManager.Instance.userData_item.GetUserMoney(), 0.3f);
        moneyAmount.text = DataManager.Instance.userData_item.GetUserMoney().ToString();
    }

    public void ShowItem()
    {
        itemInven.sprite = itemInvenSprites[DataManager.Instance.userData_item.GetUserItemCount()];
        itemAmount.text = DataManager.Instance.userData_item.GetUserItemCount().ToString();
    }

    public void MoneyInfoOnOff(bool onoff)
    {
        moneyInfo.SetActive(onoff);

        if (onoff) moneyShadow.DOFade(0.3f, 0.1f);
        else moneyShadow.DOFade(0, 0.1f);
    }

    public void ItemInfoOnOff(bool onoff)
    {
        itemInfo.SetActive(onoff);

        if (onoff) itemShadow.DOFade(0.3f, 0.1f);
        else itemShadow.DOFade(0, 0.1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Slot : MonoBehaviour
{
    public Sprite yellowBox, orrangeBox;
    public Image box, unitImage, itemImage;
    public Text number, amount, price;
    
    [HideInInspector]
    public bool isBuildable;    // 해당 슬롯 유닛의 건설가능 여부
    public Animator ani;

    private bool selected = false;
    private bool isBuildMode;
    private int index;

    private Color selectedColor = new Color(1.0f, 0.57f, 0.57f);

    private void Awake()
    {
        ani = GetComponent<Animator>();
    }

    public void ShowSlotContent(bool isBuildMode, int index)
    {
        if(this.isBuildMode == isBuildMode) ShowCount(isBuildMode, index);

        this.isBuildMode = isBuildMode;
        this.index = index;

        unitImage.gameObject.SetActive(true);
        itemImage.gameObject.SetActive(true);

        // 유닛 이미지 세팅
        int amount = DataManager.Instance.BuildableUnitCount(index);
        if (amount <= 0) unitImage.sprite = SlotManager.Instance.unitIcons_grey[index];
        else unitImage.sprite = SlotManager.Instance.unitIcons[index];

        // 유닛 가격 표시
        price.text = UnitInfoManager.Instance.unitList[index].buildMoney.ToString();
        // 돈이 부족하면 빨간색으로 표시
        if (amount == -3 || amount == -1) price.color = ColorManager.negativeRed;
        else price.color = Color.white;

        // 아이템 이미지 세팅
        int itemCount = DataManager.Instance.userData_item.ItemCount(ItemInfoManager.Instance.itemList[index].itemName);
        if (itemCount == 0) itemImage.sprite = SlotManager.Instance.itemIcons_grey[index];
        else itemImage.sprite = SlotManager.Instance.itemIcons[index];        
    }

    public void ShowCount()
    {
        if (index == -1) return;
        ShowCount(isBuildMode, index);
    }

    // 만들 수 있는 유닛의 수 or 소지 아이템 수 표시 
    public void ShowCount(bool isBuildMode, int index)
    {
        int count;

        // 빌드모드의 경우 유닛의 수
        if (isBuildMode)
        {
            count = DataManager.Instance.BuildableUnitCount(index);
            amount.text = count > 0 ? count.ToString() : "0";
        }
        // 전투모드의 경우 아이템의 수
        else
        {
            count = DataManager.Instance.userData_item.ItemCount(ItemInfoManager.Instance.itemList[index].itemName);
            amount.text = count.ToString();
        }

        isBuildable = count > 0;
    }
    
    // 슬롯 선택 및 선택해제
    public void SlotOnOff()
    {
        selected = !selected;

        if (selected)
        {
            box.sprite = orrangeBox;
            transform.DOLocalMoveY(8.0f, 0.3f);
            number.color = selectedColor;
        }
        else if (!selected)
        {
            box.sprite = yellowBox;
            transform.DOLocalMoveY(0.0f, 0.3f);
            number.color = Color.white;
        }
    }

    public void SlotInit(bool isBuildMode)
    {
        this.isBuildMode = isBuildMode;
        index = -1;
        amount.text = "";
        price.text = "";
        unitImage.gameObject.SetActive(false);
        itemImage.gameObject.SetActive(false);
    }
}

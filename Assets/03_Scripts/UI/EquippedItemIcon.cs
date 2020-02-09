using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippedItemIcon : MonoBehaviour
{
    public Image itemImage;
    public Slider slider;

    public void SetEquippedItem(UserStatusData.ItemEquip item)
    {
        if (!item.IsItemEquipped())
        {
            EmptySlot();
            return;
        }

        EquipItem(item.GetItemIndex());
        SetValue(item.GetDurability());
    }

    public void EquipItem(int index)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = SlotManager.Instance.itemIcons[index];
     
        // 임시로 수치 세팅 - 차후 기획 확정후 수정
        slider.maxValue = 100; 
        slider.value = 100;
    }

    public void SetValue(int value)
    {
        slider.value = value;
    }

    public void EmptySlot()
    {
        itemImage.gameObject.SetActive(false);
        SetValue(0);
    }
}

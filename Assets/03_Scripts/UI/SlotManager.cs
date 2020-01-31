using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    public Sprite[] unitIcons, unitIcons_grey, itemIcons, itemIcons_grey;
    public Slot[] slots;
    public Text slotGroupNumber;    // 표시되는 숏컷 세트 텍스트
    public Animator modeChanger;    // 모드체인지 아이콘 애니메이터

    [HideInInspector]
    public int[] slotGroup;
    [HideInInspector]
    public bool isBuildMode = false;

    public static SlotManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            SelectSlot(5);

        // 모드 체인지
        if (Input.GetMouseButtonDown(1)) ModeChange();

    }

    void Start()
    {
        // 데이터매니져에서 유저 슬롯 세팅 가져오기
        slotGroup = DataManager.Instance.userData_setting.GetActivatedSlotGroup();

        // 활성화된 세트넘버 표시
        slotGroupNumber.text = DataManager.Instance.userData_setting.GetActivatedSlotGroupIndex().ToString();

        // 마지막으로 선택한 슬롯 활성화
        slots[DataManager.Instance.userData_setting.GetSelectedSlot()].SlotOnOff();

        SetSlots(false);
    }

    public void ModeChange()
    {
        SetSlots(!isBuildMode);
        modeChanger.SetBool("isBuildMode", isBuildMode);
    }

    // 현재 활성화된 슬롯그룹 정보대로 슬롯ui에 정보 띄우기
    public void SetSlots(bool isBuildMode)
    {
        this.isBuildMode = isBuildMode;

        for (int i = 0; i < slots.Length; ++i)
        {
            if (slotGroup[i] == 0)
            {
                slots[i].SlotInit(isBuildMode);
                // 애니메이션 조정
                slots[i].ani.SetBool("isBuildMode", isBuildMode);
                continue;
            }
            slots[i].ShowSlotContent(isBuildMode, slotGroup[i]);
            
            // 애니메이션 조정
            slots[i].ani.SetBool("isBuildMode", isBuildMode);
        }
    }

    public void SetSlots()
    {
        SetSlots(isBuildMode);
    }

    // 슬롯그룹 변경하기(true : 인덱스 1 증가/ false : 인덱스 1 감소)
    public void SlotGroupUpDown(bool upDown)
    {
        int index = DataManager.Instance.userData_setting.GetActivatedSlotGroupIndex();

        if (upDown) index = index + 1 >= UserSettingData.slotGroupCount ? 0 : index + 1;
        if (!upDown) index = index - 1 < 0 ? UserSettingData.slotGroupCount - 1 : index - 1;

        ChangeActivatedSlotGroup(index);
    }

    // 슬롯그룹 변경
    public void ChangeActivatedSlotGroup(int number)
    {
        if (number >= UserSettingData.slotGroupCount) return;

        // 슬롯그룹 인덱스 변경 및 활성화된 슬롯 재할당
        DataManager.Instance.userData_setting.SetActivatedSlotGroupIndex(number);
        slotGroup = DataManager.Instance.userData_setting.GetActivatedSlotGroup();

        // ui 업데이트
        SetSlots(isBuildMode);
        slotGroupNumber.text = number.ToString();
    }    

    // 슬롯 선택하는 함수
    public void SelectSlot(int number)
    {
        // 같은 번호를 선택했으면 리턴
        if (number == DataManager.Instance.userData_setting.GetSelectedSlot()) return;

        // 기존에 켜져있던 슬롯 끄기
        slots[DataManager.Instance.userData_setting.GetSelectedSlot()].SlotOnOff();

        // 새 슬롯 인덱스 저장 및 켜기
        DataManager.Instance.userData_setting.SelectSlot(number);
        slots[number].SlotOnOff();
    }
}

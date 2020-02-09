using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusHudManager : MonoBehaviour
{
    // 캐릭터 아이콘
    public Slider hpBar;
    public RectTransform charaChangeCoolTime;
    public GameObject[] charaImages;

    // 체력바 관련
    public Image barFill;
    public Sprite[] barSprites;

    // 장착 아이템 관련
    public EquippedItemIcon attackItem, defendItem;

    private RectTransform hpBarRect;
    private UserStatusData.HealthInfo healthInfo;
    private Vector2 barSize, coolTimeSizeOriginal, coolTimeSize;
    private UserStatusData userStatus;

    public static StatusHudManager Instance;

    private void Awake()
    {
        Instance = this;

        hpBarRect = hpBar.gameObject.GetComponent<RectTransform>();
        barSize = hpBarRect.sizeDelta;
        coolTimeSizeOriginal = charaChangeCoolTime.sizeDelta;
        coolTimeSize = coolTimeSizeOriginal;
        coolTimeSize.y = 0;
    }

    private void Start()
    {
        userStatus = DataManager.Instance.userData_status;

        SetCharacterHud(userStatus.GetPlayingChara());
        attackItem.SetEquippedItem(userStatus.itemEquip_attack);
        defendItem.SetEquippedItem(userStatus.itemEquip_defend);
    }

    public void SetCharacterHud()
    {
        SetCharacterHud(DataManager.Instance.userData_status.GetPlayingChara());
    }

    // 해당 캐릭터에 맞게 허드 세팅
    public void SetCharacterHud(int index)
    {
        // 캐릭터 이미지 변경
        for (int i = 0; i < charaImages.Length; ++i) charaImages[i].SetActive(false);
        charaImages[index].SetActive(true);

        // 쿨타임 표시
        charaChangeCoolTime.DOSizeDelta(coolTimeSize, 0);
        charaChangeCoolTime.DOSizeDelta(coolTimeSizeOriginal, StaticValueManager.CharacterChangeCoolTime).SetEase(Ease.Linear);

        // 그린 스탠딩 or 화이트
        if (index != 2) healthInfo = DataManager.Instance.userData_status.GetHealthInfo(index);

        // 체력바 스프라이트 조정
        barFill.sprite = index == 1 ? barSprites[1] : barSprites[0];

        // 바의 길이와 값 조정
        SetBar(healthInfo.GetMaxHP(), healthInfo.GetCurrentHP());
    }

    public void SetBar(int maxHP, int currentHP)
    {
        // 바 길이 조정
        barSize.x = maxHP * 2;
        hpBarRect.DOSizeDelta(barSize, 0.3f);

        // 슬라이더 값 조정
        hpBar.maxValue = maxHP;
        hpBar.DOValue(currentHP, 0.3f);
    }
}

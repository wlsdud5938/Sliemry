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

    // 아이템 먹는 효과용 변수들
    public Transform player;
    private Camera mainCamera;
    public GameObject image;

    private ObjectPooling<Image> imagePool;
    private Vector3 playerPos, moneyPos, itemPos;

    public static InvenIconManager Instance;    

    private void Awake()
    {
        Instance = this;

        // 아이템 먹는 효과용 이미지 풀 생성
        imagePool = new ObjectPooling<Image>();
        imagePool.MakePool(transform, image, 20);
        mainCamera = FindObjectOfType<Camera>();

        moneyPos = moneyShadow.rectTransform.position;
        moneyPos.y += 44.0f;
        itemPos = itemShadow.rectTransform.position;
        itemPos.y += 44.0f;
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

    public void ItemGettingEffect(int index)
    {
        Image img;
        img = imagePool.GetObject();

        img.sprite = ItemInfoManager.Instance.itemList[index].itemSprite;
        img.SetNativeSize();

        playerPos = mainCamera.WorldToScreenPoint(player.position);
        playerPos.y += 6;

        img.rectTransform.DOMove(playerPos, 0);
        if(index == 0) img.rectTransform.DOMove(moneyPos, 1.5f).SetEase(Ease.OutQuart);
        else img.rectTransform.DOMove(itemPos, 1.5f).SetEase(Ease.OutQuart);

        StartCoroutine(GettingEffect(img.gameObject, index == 0));
    }

    private IEnumerator GettingEffect(GameObject obj, bool isMoney)
    {
        yield return new WaitForSeconds(0.8f);

        obj.SetActive(false);
        if (isMoney) ShowMoney();
        else ShowItem();
    }
}

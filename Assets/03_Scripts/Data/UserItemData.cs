using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserItemData : UserData
{
    // 유저의 아이템 데이터 클래스

    private Dictionary<string, int> userItemList;    // 유저가 가지고 있는 아이템 리스트
    private int userMoney;                           // 유저가 가지고 있는 돈(기본 정수)
    private int userItemCount;                      // 유저가 가진 아이템 수

    private const string identifier_userItem = "identifier_userItem";
    private const string identifier_userMoney = "identifier_userMoney";
    private const string identifier_userItemCount = "identifier_userItemCount";

    public static int moneyLimit = 2000;    // 소지 돈 상한
    public static int itemLimit = 20;       // 소지 아이템 상한

    public UserItemData()
    {
        userItemList = new Dictionary<string, int>();
        userMoney = 0;
        userItemCount = 0;
        LoadData();
    }

    public int GetUserMoney() { return userMoney; }

    public int GetUserItemCount() { return userItemCount; }

    public bool IsItemFull()
    {
        return userItemCount >= itemLimit;
    }

    public bool IsMoneyFull()
    {
        return userMoney >= moneyLimit;
    }

    public void EarnMoney(int amount)
    {
        userMoney += amount;
        userMoney = userMoney >= moneyLimit ? moneyLimit : userMoney;
    }

    public void UseMoney(int amount)
    {
        if (userMoney < amount) return;
        userMoney -= amount;
    }

    public void EarnItem(string itemName, int amount = 1)    // 해당 아이템과 수량 추가 및 저장
    {
        if (userItemCount + amount > itemLimit) return;     // 아이템 소지상한 넘으면 리턴

        if (userItemList.ContainsKey(itemName)) userItemList[itemName] += amount;  
        else userItemList.Add(itemName, amount);

        userItemCount += amount;    // 아이템 소지수 증가        
    }

    public bool UseItem(string itemName, int amount = 1)    // 해당 아이템을 수량만큼 사용 및 저장
    {
        if (ItemCount(itemName) == 0) return false;   // 아이템 없으면 종료

        userItemList[itemName] -= amount;
        userItemCount -= amount;    // 아이템 소지수 감소

        return true;
    }

    public int ItemCount(string itemName)    // 해당 아이템의 갯수 반환
    {
        if (!userItemList.ContainsKey(itemName)) return 0;   
        return userItemList[itemName];
    }

    public override void SaveData()
    {
        Save<Dictionary<string, int>>(identifier_userItem, userItemList);
        Save<int>(identifier_userMoney, userMoney);
        Save<int>(identifier_userItemCount, userItemCount);
    }

    public override void LoadData()
    {
        userItemList = Load<Dictionary<string, int>>(identifier_userItem);
        userMoney = Load<int>(identifier_userMoney);
        userItemCount = Load<int>(identifier_userItemCount);
    }

    public override void DeleteData()
    {
        Delete(identifier_userItem);
        Delete(identifier_userMoney);
        Delete(identifier_userItemCount);
        userItemList = new Dictionary<string, int>();
        userMoney = 0;
        userItemCount = 0;
    }
}

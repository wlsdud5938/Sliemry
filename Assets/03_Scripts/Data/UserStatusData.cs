using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStatusData : UserData
{
    [System.Serializable]
    public class HealthInfo
    {
        private int maxHP;
        private int currentHP;

        public HealthInfo(int max, int current)
        {
            maxHP = max;
            currentHP = current;
        }

        public int GetMaxHP() { return maxHP; }
        public int GetCurrentHP() { return currentHP; }
        public void SetMaxHP(int hp) { maxHP = hp; }
        public void SetCurrentHP(int hp) { currentHP = hp; }
    }

    [System.Serializable]
    public class ItemEquip
    {
        private bool isEquipped;
        private int itemIndex;
        private int durability;

        public ItemEquip()
        {
            isEquipped = false;
        }

        public void Equip(int index)
        {
            isEquipped = true;
            itemIndex = index;
            durability = 100;
        }

        public void UseItem(int cost = 1)
        {
            if (!isEquipped) return;
            durability = durability - cost > 0 ? durability - cost : 0;
            if (durability <= 0) ItemRunOut();
        }

        public void ItemRunOut()
        {
            isEquipped = false;
        }

        public bool IsItemEquipped() { return isEquipped; }
        public int GetItemIndex() { return itemIndex; }
        public int GetDurability() { return durability; }
    }

    private bool isBuildMode;
    private int playingChara;    // 0:그린, 1:화이트, 2:그린 슬라임
    public HealthInfo greenHealth, whiteHealth;
    public ItemEquip itemEquip_attack, itemEquip_defend;    // 공격 아이템과 방어 아이템 장착

    private const string identifier_isBuildMode = "identifier_isBuildMode";
    private const string identifier_greenHealth = "identifier_greenHealth";
    private const string identifier_whiteHealth = "identifier_whiteHealth";
    private const string identifier_playingChara = "identifier_playingChara";
    private const string identifier_itemEquip_attack = "identifier_itemEquip_attack";
    private const string identifier_itemEquip_defend = "identifier_itemEquip_defend";

    public UserStatusData()
    {
        isBuildMode = false;
        playingChara = 0;
        greenHealth = new HealthInfo(100, 100);
        whiteHealth = new HealthInfo(70, 70);

        itemEquip_attack = new ItemEquip();
        itemEquip_defend = new ItemEquip();

        LoadData();
    }

    public bool IsBuildMode() { return isBuildMode; }
    public int GetPlayingChara() { return playingChara; }

    public HealthInfo GetHealthInfo(int index)
    {
        if (index == 0) return greenHealth;
        else return whiteHealth;
    }

    public HealthInfo GetHealthInfo()
    {
        if (playingChara == 0) return greenHealth;
        else return whiteHealth;
    }

    // 캐릭터가 살아있는지(특정 캐릭터 확인)
    public bool IsCharacterAlive(int index)
    {
        if (index == 0) return greenHealth.GetCurrentHP() != 0;
        else if (index == 1) return whiteHealth.GetCurrentHP() != 0;

        return false;
    }

    // 캐릭터들이 살아있는지(둘 중 누구라도)
    public bool IsCharacterAlive()
    {
        return greenHealth.GetCurrentHP() != 0 || whiteHealth.GetCurrentHP() != 0;
    }

    // 다른 캐릭터가 살아있는지
    public int IsOtherCharacterAlive()
    {
        // 그린 상태에서 화이트가 죽었으면 1 리턴
        if (playingChara == 0 && !IsCharacterAlive(1)) return 1;
        // 화이트 상태에서 그린이 죽었으면 0 리턴
        if (playingChara == 1 && !IsCharacterAlive(0)) return 0;

        // 안 죽었으면 -1 리턴
        return -1;
    }

    public void BuildModeOnOff(bool onOff)
    {
        isBuildMode = onOff;
    }

    public void BuildModeOnOff()
    {
        isBuildMode = !isBuildMode;
    }

    public void SetPlayingChara(int index)
    {
        playingChara = index;
    }

    public void SetHealth(int state, int hp)
    {
        if (state == 0) greenHealth.SetCurrentHP(hp);
        if (state == 1) whiteHealth.SetCurrentHP(hp);
    }

    public void SetHealth(int hp)
    {
        if (playingChara == 0) greenHealth.SetCurrentHP(hp);
        if (playingChara == 1) whiteHealth.SetCurrentHP(hp);
    }

    public override void SaveData()
    {
        Save<bool>(identifier_isBuildMode, isBuildMode);
        Save<int>(identifier_playingChara, playingChara);
        Save<HealthInfo>(identifier_greenHealth, greenHealth);
        Save<HealthInfo>(identifier_whiteHealth, whiteHealth);
        Save<ItemEquip>(identifier_itemEquip_attack, itemEquip_attack);
        Save<ItemEquip>(identifier_itemEquip_defend, itemEquip_defend);
    }

    public override void LoadData()
    {
        isBuildMode = Load<bool>(identifier_isBuildMode);
        playingChara = Load<int>(identifier_playingChara);
        greenHealth = Load<HealthInfo>(identifier_greenHealth);
        whiteHealth = Load<HealthInfo>(identifier_whiteHealth);
        itemEquip_attack = Load<ItemEquip>(identifier_itemEquip_attack);
        itemEquip_defend = Load<ItemEquip>(identifier_itemEquip_defend);
    }

    public override void DeleteData()
    {
        Delete(identifier_isBuildMode);
        Delete(identifier_playingChara);
        Delete(identifier_greenHealth);
        Delete(identifier_whiteHealth);
        Delete(identifier_itemEquip_attack);
        Delete(identifier_itemEquip_defend);

        isBuildMode = false;
        playingChara = 0;
        greenHealth = new HealthInfo(100, 100);
        whiteHealth = new HealthInfo(70, 70);
        itemEquip_attack = new ItemEquip();
        itemEquip_defend = new ItemEquip();
    }
}

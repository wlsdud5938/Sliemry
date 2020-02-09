using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    private PlayerMove playerMoving;
    private UserStatusData userStatus;

    public void Start()
    {
        playerMoving = gameObject.GetComponent<PlayerMove>();
        userStatus = DataManager.Instance.userData_status;
        AssignCharacter();
    }

    public override void GetDamage(int attackDamage)
    {
        int damage = DamageCalculator(attackDamage);
        if (currentHP - damage > 0)
        {
            currentHP = currentHP - damage;
            userStatus.SetHealth(currentHP);
            StatusHudManager.Instance.SetBar(maxHP, currentHP);
            MessageManager.Instance.ShowMessage("데미지를 받았습니다");
        }
        else
        {
            currentHP = 0;
            userStatus.SetHealth(currentHP);
            StatusHudManager.Instance.SetCharacterHud();
            if (userStatus.GetPlayingChara() == 0) MessageManager.Instance.ShowMessage(AlarmManager.greenDown);
            if (userStatus.GetPlayingChara() == 1) MessageManager.Instance.ShowMessage(AlarmManager.whiteDown);

            // 다른 캐릭터가 살아있으면 바꾸고 아니면 게임오버
            if (userStatus.IsCharacterAlive()) playerMoving.ChangeState();
            else MessageManager.Instance.ShowMessage(AlarmManager.gameOver);
        }
        
    }

    public void AssignCharacter()
    {
        maxHP = userStatus.GetHealthInfo().GetMaxHP();
        currentHP = userStatus.GetHealthInfo().GetCurrentHP();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.CompareTag("EnemyBullet"))
            //GetDamage(other.GetComponent<Bullet>().damage);
    }
}

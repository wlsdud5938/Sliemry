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
        if (currentHP - attackDamage > 0)
        {
            currentHP = currentHP - DamageCalculator(attackDamage);
            userStatus.SetHealth(currentHP);
            StatusHudManager.Instance.SetBar(maxHP, currentHP);
        }
        else
        {
            currentHP = 0;
            userStatus.SetHealth(currentHP);
            StatusHudManager.Instance.SetCharacterHud();

            // 다른 캐릭터가 살아있으면 바꾸고 아니면 게임오버
            if (userStatus.IsCharacterAlive()) playerMoving.ChangeState();
            else Debug.Log("게임오버");
        }
        
    }

    public void AssignCharacter()
    {
        maxHP = userStatus.GetHealthInfo().getMaxHP();
        currentHP = userStatus.GetHealthInfo().getCurrentHP();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.CompareTag("EnemyBullet"))
            //GetDamage(other.GetComponent<Bullet>().damage);
    }
}

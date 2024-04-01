using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Stat
{
    public int Health;
    public int MaxHealth;

    public float MoveSpeed;
    public float RunSpeed;

    public float JumpPower;
    public float JumpConsumeStamina;

    public int Damage;

    public float Stamina;
    public int MaxStamina;
    public int RecoveryStamina;
    public int RunConsumeStamina;

    public float RotationSpeed;

    public float AttackCoolTime;
    public int AttackStamina;        // 공격 스태미나 20 필요
    public int AttackConsumeSpeed;


    public void Init()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
    }
}

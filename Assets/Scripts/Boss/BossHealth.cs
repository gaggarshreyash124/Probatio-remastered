using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    private BossAttack bossController;
    public int MaxHealth;
    public int MaxSuperarmour;
    public int MaxPosture;
    [Space]
    public float CurrentHealth;
    public float CurrentSuperarmour;
    public float CurrentPosture;
    [Space]
    public float Defence;

    private void Awake()
    {
        bossController = GetComponent<BossAttack>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
        MaxSuperarmour = MaxHealth;
        CurrentSuperarmour = MaxSuperarmour;
    }

    float CalculateDamage(float Damage)
    {
        return Damage * (100/( 100 + Defence));
    }
    
    public void TakeDamage(float damage,float posture,float weapon)
    {
        CurrentHealth -= CalculateDamage(damage);
        CurrentSuperarmour -=damage;
        
        PostureDamage(damage, weapon);
        
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("OH no Boss Dead Next part unlock");
        }
    }

    public void PostureDamage(float damage,float weapon)
    {
        if (weapon == 1)
        {
            CurrentPosture -= (MaxPosture * damage);
        }
        else if (weapon == 2)
        {
            CurrentPosture -= (MaxPosture * damage);
        }
    }
    
    public  void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
    }
}

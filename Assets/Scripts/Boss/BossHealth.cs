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
    
    bool PostureDamagetick = false;
    private bool posturelowered;
    float counter = 0;
    
    [Tooltip("amount of time before PostureDamage resets")]
    public float PostureResetCooldown = 5f;
    [Tooltip("amount of time before Super armour resets")]
    public float superArmourResetCooldown;
    
    private void Awake()
    {
        bossController = GetComponent<BossAttack>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
        MaxSuperarmour = MaxHealth;
        CurrentSuperarmour = MaxSuperarmour;
        CurrentPosture = MaxPosture;
    }

    private void Update()
    {
        if (PlayerInputHandler.Instance.grapple)
        {
            PostureDamage(10);
            PlayerInputHandler.Instance.grapple = false;
        }
        if (!posturelowered && PostureDamagetick)
        {
            posturelowered = true;
            PostureDamagetick = false;
            counter = 0;
        }
        else if (posturelowered && PostureDamagetick)
        {
            PostureDamagetick = false;
            counter = 0;
        }
        
        if (posturelowered)
        {
            counter += Time.deltaTime;
            if (counter > PostureResetCooldown)
            {
                PostureReset();
            }
        }
    }

    float CalculateDamage(float Damage)
    {
        return Damage * (100/( 100 + Defence));
    }
    
    public void TakeDamage(float Damage,float PostureDamagePercent)
    {
        CurrentHealth -= CalculateDamage(Damage);
        CurrentSuperarmour -=Damage;
        
        PostureDamage(PostureDamagePercent);
        
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("OH no Boss Dead Next part unlock");
        }
    }

    public void PostureDamage(float PostureDamagePercent)
    {
        PostureDamagetick = true;
        CurrentPosture -= (MaxPosture * PostureDamagePercent/100);
        if (CurrentPosture <= 0)
        {
            Debug.Log("Damn Boss is down time to repost");
        }
        Debug.Log(CurrentPosture);
        Debug.Log(PostureDamagePercent/100);
    }

    public void PostureReset()
    {
        posturelowered = false;
        CurrentPosture = MaxPosture;
    }
    
    public  void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
    }
}

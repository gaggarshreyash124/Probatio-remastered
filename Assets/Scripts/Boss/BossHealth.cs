using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    private BossAttack bossController;
    public Animator BossAnimator;
    
    
    public int MaxHealth;
    public int MaxSuperArmour;
    public int MaxPosture;
    [Space]
    public float CurrentHealth;
    public float CurrentSuperArmour;
    public float CurrentPosture;
    [Space]
    public float Defence;
    
    float Posturecounter = 0;
    private float SuperCounter = 0;
    
    private bool PostureDamagetick = false;
    private bool posturelowered;
    
    private bool SuperArmourDamagetick = false;
    private bool SuperArmourlowered;
    public float JuampAttackDistanceMultiplier;
    [Tooltip("amount of time before PostureDamage resets")]
    public float PostureResetCooldown = 5f;
    [Tooltip("amount of time before Super armour resets")]
    public float superArmourResetCooldown;
    float c = 0;
    private void Awake()
    {
        bossController = GetComponent<BossAttack>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
        MaxSuperArmour = MaxHealth;
        CurrentSuperArmour = MaxSuperArmour;
        CurrentPosture = MaxPosture;
    }

    private void Update()
    {
        
        if (PlayerInputHandler.Instance.CrounchInput)
        {
            PostureDamage(10);
            
            PlayerInputHandler.Instance.CrounchInput = false;
        }

        if (PlayerInputHandler.Instance.CrounchInput)
        {
            TakeDamage(15,5);
            PlayerInputHandler.Instance.CrounchInput = false;
        }

        if (PlayerInputHandler.Instance.BossJumpInput)
        {
            BossAnimator.Play("Jump Attack");
        }
        PostureReset();
        SuperArmourReset();
        
        c+= Time.deltaTime;
        Debug.Log(c);
        if (c >= 5)
        {
            c = 0;
            BossAnimator.SetTrigger("Attack");
        }
        
    }

    float CalculateDamage(float Damage)
    {
        return Damage * (100/( 100 + Defence));
    }
    
    public void TakeDamage(float Damage,float PostureDamagePercent)
    {
        SuperArmourDamagetick = true;
            
        CurrentHealth -= CalculateDamage(Damage);
        CurrentSuperArmour -=Damage;
        Debug.Log("Health" +  CurrentHealth);
        Debug.Log("Posture" + CurrentPosture);
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
        Debug.Log("Posture " +CurrentPosture);
        //Debug.Log(MaxPosture * PostureDamagePercent/100);
    }

    public void PostureReset()
    {
        if (!posturelowered && PostureDamagetick)
        {
            posturelowered = true;
            PostureDamagetick = false;
            Posturecounter = 0;
        }
        else if (posturelowered && PostureDamagetick)
        {
            PostureDamagetick = false;
            Posturecounter = 0;
        }
        
        if (posturelowered)
        {
            Posturecounter += Time.deltaTime;
            if (Posturecounter > PostureResetCooldown)
            {
                posturelowered = false;
                CurrentPosture = MaxPosture;
            }
        }
    }

    public void SuperArmourReset()
    {
        if (!SuperArmourlowered && SuperArmourDamagetick)
        {
            SuperArmourlowered = true;
            SuperArmourDamagetick = false;
            SuperCounter = 0;
        }
        else if (SuperArmourlowered && SuperArmourDamagetick)
        {
            SuperArmourDamagetick = false;
            SuperCounter = 0;
        }
        
        if (SuperArmourlowered)
        {
            SuperCounter += Time.deltaTime;
            if (SuperCounter > superArmourResetCooldown)
            {
                SuperArmourlowered = false;
                CurrentSuperArmour = MaxSuperArmour;
            }
        }
    }

    private void OnAnimatorMove()
    {
        AnimatorStateInfo Sate = BossAnimator.GetCurrentAnimatorStateInfo(0);

        if (Sate.IsName("Jump Attack"))
        {
            Vector3 DeltaPosition = BossAnimator.deltaPosition;

            DeltaPosition *= JuampAttackDistanceMultiplier;
        }
        
        transform.position += BossAnimator.deltaPosition * Time.deltaTime;
        
    }

    public  void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
    }
}

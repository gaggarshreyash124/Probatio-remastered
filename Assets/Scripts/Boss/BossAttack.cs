using UnityEngine;

public class BossAttack : BossPlayerDetection
{
    PlayerHealth playerHealth;
    public bool isAttacking;
    public bool Frenzy = false;

    private bool canattackpunch;
    private bool canattacksmash;
    private bool canattackjump;
    private bool canattacksweep;
        
    public override void Update()
    {
        base.Update();

        switch (BossPlayerRange)
        {
            case Range.small:
                break;
            case Range.medium:
                break;
            case Range.big:
                break;
        }
        
        
        
    }

    public void basicPunch()
    {
        
    }

    public void SmashAttack()
    {
        
    }

    public void JumpAttack()
    {
        
    }

    public void SweepAttack()
    {
        
    }
    
}

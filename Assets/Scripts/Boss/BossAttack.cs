using UnityEngine;

public class BossAttack : BossPlayerDetection
{
    PlayerHealth playerHealth;
    public bool isAttacking;
    public bool Frenzy = false;

    public override void Update()
    {
        base.Update();
        if (inSmallRange())
        {
            basicPunch();
        }
        else if (inMidRange())
        {
            SmashAttack();
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

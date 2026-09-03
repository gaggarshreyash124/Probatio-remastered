using System.Collections.Generic;
using UnityEngine;


public class BossAttack : MonoBehaviour
{
    PlayerHealth playerHealth;
    
    bool iscombofinished = false;
    
    public bool Frenzy = false;
        
    public void Update()
    {
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

[System.Serializable]
public class Attacks
{
    public List<Boss1Attacks> ComboAttacks;
}

public enum Boss1Attacks
{
    basicPunch,
    Smash,
    Jump,
    Sweep,
    Delay,
    Grace
}

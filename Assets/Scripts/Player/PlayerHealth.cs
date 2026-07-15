using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth playerHealth;
    public bool Dead;
    public int MaxHealth;
    public float CurrentHealth;

    private void Awake()
    {
        playerHealth = this;
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Dead = true;
        }
    }
}

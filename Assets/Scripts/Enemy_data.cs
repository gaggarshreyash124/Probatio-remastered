using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_data", menuName = "Scriptable Objects/Enemy_data")]
public class Enemy_data : ScriptableObject
{
    [Header("Enemy Type")] public bool LungeEnemy = false;
    
    [Header("Max Health")] public float maxHealth = 100f;
    
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    
    [Header("Detection Ranges")]
    public float walkRange = 10f;
    public float runRange = 30f;
    public float maxRange = 40f;
    public float minRange = 20f;
    public float CheckRange = 50f;
    public float AngleCheck = 60f;
    
    [Header("Behind Wall Check Times")]
    public float walkTime = 1.5f;
    public float runTime = 0.5f;
    
    [Header("Player Mask")]
    public LayerMask player_mask;
}

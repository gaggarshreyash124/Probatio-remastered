using System;
using UnityEngine;
using UnityEngine.AI;

public class BossPlayerDetection: MonoBehaviour
{
    public bool StartFight;
    
    Vector3 PlayerPosition;
    
    public NavMeshAgent agent;

    public float smallrange;
    public float mediumrange;
    public float bigrange;
    
    bool inSmallRange()
    {
        return Vector3.Distance(PlayerPosition, transform.position) <= smallrange;
    }

    bool MidRange()
    {
        return Vector3.Distance(PlayerPosition, transform.position) <= mediumrange;
    }

    bool BigRange()
    {
        return Vector3.Distance(PlayerPosition, transform.position) <= bigrange;
    }
    
    private void Start()
    {
        PlayerPosition = PlayerMovementStates.Player.transform.position;
    }

    private void Update()
    {
        PlayerPosition = PlayerMovementStates.Player.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, smallrange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, mediumrange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, bigrange);
    }
}

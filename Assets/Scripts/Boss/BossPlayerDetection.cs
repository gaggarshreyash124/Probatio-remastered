using System;
using UnityEngine;
using UnityEngine.AI;

public class BossPlayerDetection: MonoBehaviour
{
    protected Vector3 PlayerPosition;
    
    public NavMeshAgent agent;

    public float smallrange;
    public float mediumrange;
    public float bigrange;
    
    protected bool inSmallRange()
    {
        return Vector3.Distance(PlayerPosition, transform.position) <= smallrange;
    }

    protected bool inMidRange()
    {
        return Vector3.Distance(PlayerPosition, transform.position) <= mediumrange;
    }

    protected bool inBigRange()
    {
        return Vector3.Distance(PlayerPosition, transform.position) <= bigrange;
    }
    
    public void Start()
    {
        PlayerPosition = PlayerMovementStates.Player.transform.position;
    }

    public virtual void Update()
    {
        PlayerPosition = PlayerMovementStates.Player.transform.position;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, smallrange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, mediumrange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, bigrange);
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Base_enemy_movement : Enemy_checks
{
    [SerializeField] float walk_speed;
    [SerializeField] float sprint_speed;
    [SerializeField] float attack_range;
    [SerializeField] List<Transform> PatrolPoints = new List<Transform>();
    int current_point;

    void Start()
    {
        agent.stoppingDistance = 0.2f;
    }

    private void Update()
    {
        if (in_range())
        {
            follow_player();
            Debug.Log("Follow Player");
            Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        }
        else if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Patrol");
            StartCoroutine(Patrol());
        }
    }

    void follow_player()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > attack_range)
        {
            if (PlayerStats == null)
            {
                agent.speed = sprint_speed;
            }
            else
            {
                agent.speed = walk_speed;
            }

            agent.destination = player.transform.position;
        }
        else
        {
            attack_player();
        }
    }

    void attack_player()
    {
        agent.ResetPath();
    }

    IEnumerator Patrol()
    {
        agent.ResetPath();
        ShiftMovement();
        yield return new WaitForSeconds(2);
    }

    void ShiftMovement()
    {
        if (PatrolPoints.Count == 0)
        {
            return;
        }

        agent.SetDestination(PatrolPoints[current_point].position);
        Debug.Log(current_point);
        current_point = (current_point + 1) % PatrolPoints.Count;

    }
}
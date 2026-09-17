using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Base_enemy : Enemy_checks
{
    [SerializeField] float attack_range;
    [SerializeField] List<Transform> PatrolPoints = new List<Transform>();
    int current_point;
    Animator anim;
    private float health;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent.stoppingDistance = 0.2f;
        health = data.maxHealth;
    }

    private void Update()
    {
        anim.SetBool("Damage", false);
        if (in_range())
        {
            follow_player();
        }
        else if (!(agent.hasPath || agent.pathPending) || (agent.remainingDistance <= agent.stoppingDistance))
        {
            StartCoroutine(Patrol());
        }
    }

    void follow_player()
    {
        
        if (Vector3.Distance(transform.position, player.transform.position) > attack_range)
        {
            anim.SetBool("Attack", false);
            anim.SetBool("Walk", true);
            switch (state_check.isSprinting)
            {
                case true:
                    agent.speed = data.runSpeed;
                    break;
                case false:
                    agent.speed = data.walkSpeed;
                    break;
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
        anim.SetBool("Walk", false);
        anim.SetBool("Attack", true);
    }

    IEnumerator Patrol()
    {
        anim.SetBool("Walk", false);
        yield return new WaitForSeconds(2);
        anim.SetBool("Walk", true);
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
        current_point = (current_point + 1) % PatrolPoints.Count;
        Debug.Log(current_point);
    }

    public void TakeDamage(int damage)
    {
        anim.SetBool("TakeDamage",true);
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Death");
            gameObject.SetActive(false);
        }
    }
}
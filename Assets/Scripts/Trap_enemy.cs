using System;

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Trap_enemy : Enemy_checks
{
    public float DistToGround = 5;
    private float speed = 0;
    private bool fall = false;
    [SerializeField] private LayerMask ground_mask;
    private Animator animator;
    void Start()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (in_range())
        {
            if (Physics.Raycast(transform.position, Vector3.down, DistToGround, data.player_mask))
            {
                fall = true;
                animator.SetBool("Found", true);
            }
        }
        if (fall)
        {
            if (gameObject.GetComponent<NavMeshAgent>().enabled == false)
            {
                PlayerCheck();
            }
            else
            {
                FollowPlayer();
            }
        }
    }
    private void PlayerCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f, ground_mask))
        {
            speed = 0;
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            animator.SetBool("Chase", true);
            
        }
        else if (Physics.Raycast(transform.position, Vector3.down, 0.5f, data.player_mask))
        {
            speed = 0;
            transform.position = player.transform.position + (Vector3.up *1.8f);
            animator.SetBool("Grab", true);
        }
        else
        {
            if (speed <= data.GravSpeed)
            {
                speed += Time.deltaTime * data.GravSpeed;
            }
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    void FollowPlayer()
    {
        agent.speed = data.walkSpeed;
        agent.destination = player.transform.position;
    }
}

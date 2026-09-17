using UnityEngine;
using UnityEngine.AI;

public class Enemy_checks : MonoBehaviour
{
   protected Transform player;
   protected PlayerMovementState state_check;
   [SerializeField] protected Enemy_data data;
   [SerializeField] protected NavMeshAgent agent;
   float current_time = 0;
   protected bool in_range()
   {
      Collider[] present = Physics.OverlapSphere(transform.position, data.CheckRange, data.player_mask);
      if (present.Length != 0)
      {
         player = present[0].transform;
         state_check = player.gameObject.GetComponent<PlayerMovementState>();
         if (Vector3.Distance(transform.position, player.transform.position) < data.minRange)
         {
            return walk_check();
         }
         if (Vector3.Distance(transform.position, player.transform.position) < data.maxRange)
         {
            return (fov_check() || sprint_check());
         }
      }
      return false;
   }

   protected bool fov_check()
   {
      float direction = Vector3.Angle(transform.forward, player.transform.position - transform.position);
      if (direction < data.AngleCheck)
      {
         if (!(Physics.Raycast(transform.position, player.transform.position, data.maxRange, data.player_mask)) && !(agent.hasPath))
         {
            return behind_wall_check();
         }
         return true;
      }
      return false;
   }

   protected bool sprint_check()
   {
      if ((Vector3.Distance(transform.position, player.transform.position) <= data.runRange))
      {
         return true;
      }
      return false;
   }

   protected bool walk_check()
   {
      if (Vector3.Distance(transform.position, player.transform.position) <= data.walkRange)
      {
         return true;
      }
      return false;
   }

   protected bool behind_wall_check()
   {
      current_time += Time.deltaTime;
      if (state_check.isSprinting)
      {
         if (current_time > data.runTime)
         {
            current_time = 0;
            return true;
         }
         return false;
      }
      if (current_time > data.walkTime)
      {
         current_time = 0;
         return true;
      }
      return false;
   }
}

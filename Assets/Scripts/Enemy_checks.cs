using UnityEngine;
using UnityEngine.AI;

public class Enemy_checks : MonoBehaviour
{
    protected Transform player;
   [SerializeField] protected NavMeshAgent agent;
   [SerializeField] protected float walk_range;
   [SerializeField] protected float sprint_range;
   [SerializeField] protected float fov_range;
   [SerializeField] protected float max_range;
   [SerializeField] protected float min_range;
   [SerializeField] protected float walk_time;
   [SerializeField] protected float sprint_time;
   [SerializeField] protected LayerMask player_mask;
   protected ScriptableObject PlayerStats;
   protected float current_time = 0;
   protected bool in_range()
   {
      Collider[] present = Physics.OverlapSphere(transform.position, fov_range, player_mask);
      if (present.Length != 0)
      {
         player = present[0].transform;
         PlayerStats = player.gameObject.GetComponent<ScriptableObject>();
         if (Vector3.Distance(transform.position, player.transform.position) < min_range)
         {
            return walk_check();
         }
         if (Vector3.Distance(transform.position, player.transform.position) < max_range)
         {
            return (fov_check() || sprint_check());
         }
      }
      return false;
   }

   protected bool fov_check()
   {
      float direction = Vector3.Angle(transform.forward, player.transform.position - transform.position);
      if (direction < fov_range)
      { 
         if (!(Physics.Raycast(transform.position, player.transform.position, sprint_range, player_mask)) && !(agent.hasPath))
         {
            return behind_wall_check();
         }
         return true;
      }
      return false;
   }

   protected bool sprint_check()
   {
      if ((Vector3.Distance(transform.position, player.transform.position) <= sprint_range) && PlayerStats)
      {
         return true;
      }
      return false;
   }

   protected bool walk_check()
   {
      if (Vector3.Distance(transform.position, player.transform.position) <= walk_range)
      {
         return true;
      }
      return false;
   }

   protected bool behind_wall_check()
   {
      current_time += Time.deltaTime;
      if (true)
      {
         if (current_time > sprint_time)
         {
            current_time = 0;
            return true;
         }
         return false;
      }
      if (current_time > walk_time)
      { current_time = 0;
            return true;
      }
      return false;
   }
}

using UnityEngine;
using UnityEngine.AI;

public class BossAttack : MonoBehaviour
{
    BossHealth  BossHealth;
    
    GameObject Player;
    Vector3 PlayerPosition;
    
    NavMeshAgent Bossagent;
    NavMeshTriangulation BossNavMesh;
    Animator BossAnimator;
    
    public float Movespeed;
    public float ChargeSpeed;
    public float Delay;

    [SerializeField] private bool Spawned;
    private float counter = 0;
    
    [Header("Flee Distance")]
    public float MinActionDesionDistance = 8f;
    public float MaxActionDesionDistance = 12f;

    [Header("Direction")]
    public int directionCount = 16;
    public float distancePriority = 0.5f;

    [Header("NavMesh")]
    public float secondarywalkableCheck = 2f;

    [Header("Wall Detection")]
    public LayerMask GroundMask;
    public float wallcheckradius = 5f;
    public float walkableAngle = 45f;
    public float prioritysub = 1.5f;

    [Header("Random Offset")]
    public float offsetAmount = 3f;
    
    Vector3 Offset()
    {
        float x = Random.Range(-offsetAmount, offsetAmount);
        float z = Random.Range(-offsetAmount, offsetAmount);

        return new Vector3(x, 0f, z);
    }
    int UnwalkableSurfaceCount(Vector3 point)
    {
        Collider[] hits = Physics.OverlapSphere(
            point,
            wallcheckradius,
            GroundMask
        );

        int unwalkableCount = 0;

        foreach (Collider hit in hits)
        {
            Vector3 closestPoint = hit.ClosestPoint(point);

            Vector3 direction = point - closestPoint;

            if (direction.sqrMagnitude < 0.001f)
                continue;

            direction.Normalize();

            float angle = Vector3.Angle(Vector3.up, direction);

            if (angle > walkableAngle)
            {
                unwalkableCount++;
            }
        }

        return unwalkableCount;
    }
    Vector3 BestPosition()
    {
        Vector3 BosssDirection = transform.position - PlayerPosition;

        BosssDirection.y = 0f;

        if (BosssDirection.sqrMagnitude < 0.01f)
            return transform.position;

        BosssDirection.Normalize();
        
        float bestScore = float.MinValue;

        Vector3 bestPosition = transform.position;
        
        float randomAngleOffset = Random.Range(0f, 360f);

        for (int i = 0; i < directionCount; i++)
        {
            float randomDistance = Random.Range(
                MinActionDesionDistance,
                MaxActionDesionDistance
            );
            
            float angle =
                randomAngleOffset +
                (360f / directionCount) * i;

            Vector3 PossibleDirection =
                Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            
            Vector3 posiblePosition =
                PlayerPosition +
                PossibleDirection * randomDistance;
            
            Vector3 DisplacementPosition =
                posiblePosition + Offset();
            
            if (!NavMesh.SamplePosition(
                DisplacementPosition,
                out NavMeshHit hit,
                secondarywalkableCheck,
                NavMesh.AllAreas))
            {
                continue;
            }
            
            NavMeshPath path = new NavMeshPath();

            if (!Bossagent.CalculatePath(hit.position, path))
            {
                continue;
            }

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                continue;
            }

            float directionScore =
                Vector3.Dot(
                    PossibleDirection,
                    BosssDirection
                );

            float distanceScore = Mathf.InverseLerp(
                MinActionDesionDistance,
                MaxActionDesionDistance,
                randomDistance
            );

            directionScore +=
                distanceScore * distancePriority;

            int unwalkableCount =
                UnwalkableSurfaceCount(hit.position);
            
            directionScore -=
                unwalkableCount * prioritysub;

            if (directionScore > bestScore)
            {
                bestScore = directionScore;
                bestPosition = hit.position;
            }
        }

        return bestPosition;
    }

    private void Awake()
    {
        BossHealth = GetComponent<BossHealth>();
        BossAnimator = GetComponent<Animator>();
        Bossagent = GetComponent<NavMeshAgent>();
        
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        if (!Spawned && !BossHealth.BossDead)
        {
            Spawned =  true;
        }
    }

    private void Update()
    {
        if (Bossagent.speed != Movespeed)
        {
            Bossagent.speed = Movespeed;
        }
        counter += Time.deltaTime;
        PlayerPosition = Player.transform.position;

        if (counter >= Delay)
        {
            counter = 0;
            Bossagent.SetDestination(BestPosition());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinActionDesionDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, MaxActionDesionDistance);
        
        
    }
}
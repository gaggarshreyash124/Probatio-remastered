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
    public float Delay;

    [SerializeField] private bool Spawned;
    
    [Header("Flee Distance")]
    public float MinActionDesionDistance = 8f;
    public float MaxActionDesionDistance = 12f;
    public float minTravelDistance = 5f;

    [Header("Direction")]
    public int directionCount = 16;
    public float distancePriority = 0.2f;

    [Header("NavMesh")]
    public float secondarywalkableCheck = 2f;
    private NavMeshHit hit;

    [Header("Wall Detection")]
    public LayerMask GroundMask;
    public float wallcheckradius = 5f;
    public float walkableAngle = 45f;

    [Header("Random Offset")]
    public float offsetAmount = 3f;
    
    [Header("Rotation")]
    public bool FacePlayer = true;
    public float rotationSpeed = 15f;
    public float minfovangle = 15f;

    
    [Header("Direction Reapet Managment")] 
    public Vector3 PreviousBestPosition;
    public bool haspreviousvalue;
    
    [Header("decision States")]
    public BossDecisionStates AttackStates;

    public bool Allattackscooldown;

    [Header("Grace Period")] 
    public bool GracePeriodCooldown;
    public float GraceTime;
    float c = 0;

    [Header("Charge State")] 
    public float ChargeSpeed => Movespeed * 1.5f;
    bool gotposition;
    Vector3 position;
    
    bool canCharge()
    {
        return Vector3.Distance(PlayerPosition,transform.position) > MinActionDesionDistance - 0.5f && Allattackscooldown;
    }

    bool needDistance()
    {
        return Vector3.Distance(PlayerPosition, transform.position) < MinActionDesionDistance && Allattackscooldown && GracePeriodCooldown;
    }
    
    Vector3 Offset() 
    { 
        float x = Random.Range(0, 3);
        float z = Random.Range(0, 3);
        
        return new Vector3(x, 0, z); 
    }

    public bool HasUnwalkableSurface(Vector3 point)
    {
        Collider[] hits = Physics.OverlapSphere( point, wallcheckradius, GroundMask ); 
        
        foreach (Collider hit in hits) 
        { 
            Vector3 direction = (point - hit.ClosestPoint(point)).normalized;
            float angle = Vector3.Angle(Vector3.up, direction);
            if (angle > walkableAngle) return true;
        } 
        return false;
    }
    
    bool IsPositionValid(Vector3 position, out Vector3 validPosition)
    {
        bool isValid = NavMesh.SamplePosition(position, out NavMeshHit navHit, secondarywalkableCheck, NavMesh.AllAreas);
        validPosition = isValid ? navHit.position : Vector3.zero;
        return isValid;
    }

    bool TryFindValidPositionInDirection(Vector3 direction, float minDist, float maxDist, float step, out Vector3 result)
    {
        for (float dist = minDist; dist <= maxDist; dist += step)
        {
            Vector3 candidate = PlayerPosition + direction * dist;
            if (IsPositionValid(candidate, out Vector3 valid))
            {
                result = valid;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    Vector3 BestPosition()
    {
        Vector3 BosssDirection = transform.position - PlayerPosition;
        BosssDirection.y = 0f;
        
        if (BosssDirection.sqrMagnitude < 0.01f) return transform.position;
        
        BosssDirection.Normalize();

        float bestScore = float.MinValue;
        Vector3 bestPosition = transform.position;

        for (int i = 0; i < directionCount; i++)
        {
            float angle = (360f / directionCount) * i;
            Vector3 PossibleDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            if (!TryFindValidPositionInDirection(PossibleDirection, MinActionDesionDistance, MaxActionDesionDistance, 1, out Vector3 posiblePosition))
            {
                continue; 
            }

            Vector3 finalPosition = posiblePosition + Offset();
            
            NavMeshPath path = new NavMeshPath();
            if (!Bossagent.CalculatePath(finalPosition, path) || path.status != NavMeshPathStatus.PathComplete)
            {
                continue;
            }

            float directionScore = Vector3.Dot(PossibleDirection, BosssDirection);

            if (HasUnwalkableSurface(finalPosition))
            {
                directionScore -= distancePriority;
            }

            if (haspreviousvalue)
            {
                Vector3 previousDirection = (PreviousBestPosition - PlayerPosition).normalized;
                Vector3 newDirection = (finalPosition - PlayerPosition).normalized;
                float dot = Vector3.Dot(previousDirection, newDirection);

                if (dot > 0.99f)
                {
                    directionScore -= distancePriority;
                }

                if (Vector3.Distance(PreviousBestPosition, finalPosition) < minTravelDistance)
                {
                    continue;
                }
            }

            if (directionScore > bestScore)
            {
                bestScore = directionScore;
                bestPosition = finalPosition;
            }
        }

        PreviousBestPosition = bestPosition;
        haspreviousvalue = true;

        return bestPosition;
    
    }

    private void Awake()
    {
        BossHealth = GetComponent<BossHealth>();
        BossAnimator = GetComponent<Animator>();
        Bossagent = GetComponent<NavMeshAgent>();
        AttackStates = BossDecisionStates.None;
        
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
        PlayerPosition = Player.transform.position;
        if (!Mathf.Approximately(Bossagent.speed, Movespeed))
        {
            Bossagent.speed = Movespeed;
        }

        currentStateCheck();
        Rotatetofaceplayer();

        if (GracePeriodCooldown)
        {
            c += Time.deltaTime;
            
            if (c >= 10f)
            {
                c = 0;
                GracePeriodCooldown = false;
            }
        }
    }

    public void currentStateCheck()
    {
        switch (AttackStates)
        {
            case BossDecisionStates.GracePeriod:
                GracePeriod();
                break;
            case BossDecisionStates.Charge:
                Charge();
                break;
            case BossDecisionStates.Retreat:
                Retreat();
                break;
            case BossDecisionStates.None:
                DecideState();
                break;
        }
    }

    public void StartRotate() => FacePlayer = true;
    public void Rotatetofaceplayer()
    {
        if (!FacePlayer) return;
        
        Quaternion TargetDirection = Quaternion.LookRotation(PlayerPosition - transform.position);
        float angletotarget = Quaternion.Angle(transform.rotation, TargetDirection);

        if (angletotarget < minfovangle && angletotarget > -minfovangle)
        {
            Debug.Log(angletotarget);
            FacePlayer = false;
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetDirection, rotationSpeed * Time.deltaTime);
    }

    public void DecideState()
    {
        if (!GracePeriodCooldown)
        {
            AttackStates = BossDecisionStates.GracePeriod;
            
        }
        else if (needDistance())
        {
            StartRotate();
            AttackStates = BossDecisionStates.Retreat;
        }
        else if (canCharge())
        {
            AttackStates = BossDecisionStates.Charge;
        }
        else
        {
            StartRotate();
        }
    }

    public void Charge()
    {
        if (!gotposition)
        {
            position = PlayerPosition;
            gotposition = true;
        }
        Bossagent.speed = ChargeSpeed;
        
        Bossagent.SetDestination(position);
        NavMeshPath path = new NavMeshPath();
        if (!Bossagent.CalculatePath(position, path))
        {
            Debug.Log("Path failed,cant Charge");
            AttackStates =  BossDecisionStates.None;
        }
        else if (path.status == NavMeshPathStatus.PathComplete)
        {
            AttackStates =  BossDecisionStates.None;
            Debug.Log("Chaaaaaarge");

        }
    }

    public void GracePeriod()
    {
        GraceTime -= Time.deltaTime;

        if (GraceTime <= 0)
        {
            GracePeriodCooldown = true;
            AttackStates =  BossDecisionStates.None;
        }
            
    }

    public void Retreat()
    {
        Bossagent.SetDestination(BestPosition());
        NavMeshPath path = new NavMeshPath();
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            AttackStates = BossDecisionStates.None;

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

public enum BossDecisionStates
{
    None,
    GracePeriod,
    Attack,
    Charge,
    Retreat
}
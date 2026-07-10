using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovementStates : MonoBehaviour
{
#region Movement variables

    public CharacterController controller;
    public CinemachineCamera cam;
    public Animator anim;
    
    Vector3 velocity;
    
    public float Gravity = -9.81f;
    public float airresistance;
    public float Weight = 80f;
    public float area;
    
    public float WalkSpeed;
    public float RunSpeed;
    public float RotationSpeed = 10f;
    float xcurrent;
    float zcurrent;
    public float TransitionSpeed;
    
    public bool isGrounded;
    public bool wasGrounded;
    
    private float airTime;
    public float airactioninterval = 0.6f;
    
    public bool isSprinting;
    private bool isWalking;
    private bool isIdle;
    
    float CurrentSpeed()
    {
        float speed = isSprinting ? RunSpeed : WalkSpeed;
        float massMultiplier = 1f / (1f + Weight * 0.01f);
        speed *= massMultiplier;
        return speed;
    }
    Vector3 CamForward()
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0f; 
        return forward.normalized;
    }
    Vector3 CamRight()
    {
        Vector3 right = cam.transform.right;
        right.y = 0f;
        return right.normalized;
    }
    Vector3 moveDir => CamForward() * PlayerInputHandler.Instance.MoveInput.y + CamRight() * PlayerInputHandler.Instance.MoveInput.x;
    
    private float TerminalVelocity()
    {
        float airDensity = 1.225f;
        return Mathf.Sqrt((2f * Weight * Mathf.Abs(Gravity))/ (airDensity * airresistance * area));
    }

    private States PrevSuperStates,CurrentSuperStates;
    private GroundedStates CurrentGroundedStates;
    private AbilityStates currentAbilityStates;
    
    bool canEnterAirState => AnimationFinished;
    public bool AnimationFinished;
    
    bool ShouldEnterIdle()
    {
        return PlayerInputHandler.Instance.MoveInput == Vector2.zero;
    }
    bool ShouldEnterWalk()
    {
        return PlayerInputHandler.Instance.MoveInput != Vector2.zero && !PlayerInputHandler.Instance.SprintInput;
    }
    bool ShouldEnterRun()
    {
        return PlayerInputHandler.Instance.MoveInput != Vector2.zero && PlayerInputHandler.Instance.SprintInput;
    }
    
    public float ActionInterval;
    public bool ActionEnd;
    public float ActionEndTime;

    public bool CanDodge = true;
    public float DodgeAbilityCooldown = .5f;

    public bool CanLeap;
    public float LeapAbilityCooldown;

    public bool inAir;
    GroundedStates DecideGroundedState()
    {

        if (ShouldEnterRun())
            return GroundedStates.Run;

        if (ShouldEnterWalk())
            return GroundedStates.Walk;

        return GroundedStates.Idle;
    }
    
#endregion
    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        ChangeGroundedStates(GroundedStates.Transition);
        CurrentSuperStates = States.Grounded;
        currentAbilityStates = AbilityStates.None;
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        if (wasGrounded && !isGrounded)
        {
            airTime = airactioninterval;
            wasGrounded = false;
            
        }
        else if (!wasGrounded && isGrounded)
        {
            wasGrounded = true;
            inAir = false;
            anim.SetBool("Inair", false);
        }
        
        anim.SetBool("Grounded", wasGrounded);
        
        AbilityCallInputs();
        HandleSuperTransition();
        HandleSuperStates();

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += Gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -TerminalVelocity(), Mathf.Infinity);

        controller.Move(Vector3.up * velocity.y * Time.deltaTime);

        if (!wasGrounded && !inAir)
        {
            Debug.Log("StartAirCount");
            airTime -= Time.deltaTime;
            Debug.Log(airTime);
            if (airTime <= 0)
            {
                inAir = true;
                anim.SetBool("Inair", true);
            }
        }
    }
    
#region Super States    

    void HandleSuperStates()
    {
        switch (CurrentSuperStates)
        {
            case States.Grounded:
                Debug.Log("Ground State");
                HandleGroundedStates();
                break;

            case States.Abilities:
                Debug.Log("Ability State");
                HandleAbilityState();
                break;

            case States.InAir:
                Debug.Log("Inair State");
                break;
            
            case States.Combat:
                AttackCallInputs();
                break;
        }
    }

    void HandleSuperTransition()
    {
        if (currentAbilityStates != AbilityStates.None)
        {
            CurrentSuperStates = States.Abilities;
        }
        else if (!isGrounded && canEnterAirState)
        {
            CurrentSuperStates = States.InAir;
        }
        else if (PlayerInputHandler.Instance.AttackInput || inCombat)
        {
            CurrentSuperStates = States.Combat;
        }
        else
        {
            CurrentSuperStates = States.Grounded;
        }
    }
#endregion    

#region Grounded States
    
    void HandleGroundedStates()
    {
        switch (CurrentGroundedStates)
        {
            case GroundedStates.Idle:
                HandleGroundedIdle();
                break;
            case GroundedStates.Walk:
                HandleGroundedWalk();
                break;
            case GroundedStates.Run:
                HandleGroundedRun();
                break;
            case GroundedStates.Land:
                HandleGroundedLand();
                break;
            case GroundedStates.Transition:
                TransitionGroundedState();
                break;
        }
    }

    void EnterGroundedStates()
    {
        switch (CurrentGroundedStates)
        {
            case GroundedStates.Idle:
                HandleGroundedIdleEnter();
                break;
            case GroundedStates.Walk:
                HandleGroundedWalkEnter();
                break;
            case  GroundedStates.Run:
                HandleGroundedRunEnter();
                break;
            case GroundedStates.Transition:
                break;
        }
    }
    void ExitGroundedStates()
    {
        switch (CurrentGroundedStates)
        {
            case GroundedStates.Idle:
                HandleGroundedIdleExit();
                break;
            case GroundedStates.Walk:
                HandleGroundedWalkExit();
                break;
            case GroundedStates.Run:
                HandleGroundedRunExit();
                break;
            case GroundedStates.Transition:
                break;
        }
    }

    void TransitionGroundedState()
    {
        GroundedStates next = DecideGroundedState();

        ChangeGroundedStates(next);
    }
    void ChangeGroundedStates(GroundedStates newGroundedStates)
    {
        ExitGroundedStates();
        
        CurrentGroundedStates = GroundedStates.None;
        
        CurrentGroundedStates = newGroundedStates;
        
        EnterGroundedStates();
    }

    void HandleGroundedIdleEnter()
    {
        isIdle = true;
        anim.SetBool("Idle", isIdle);
    }
    void HandleGroundedIdle()
    {
        if (!ShouldEnterIdle()) ChangeGroundedStates(GroundedStates.Transition);

    }
    void HandleGroundedIdleExit()
    {
        isIdle = false;
        anim.SetBool("Idle", isIdle);
    }
    
    void HandleGroundedWalkEnter()
    {
        isWalking = true;
        anim.SetBool("Walk", isWalking);
    }
    void HandleGroundedWalk()
    {
        if (ShouldEnterWalk() && ActionEnd)
        {
            ActionEnd = false;
        }
        else if (!ShouldEnterWalk() && !ActionEnd)
        {
            ActionEnd = true;
            ActionEndTime = Time.time;
        }
        
        if (!ShouldEnterWalk() && ActionEndTime < Time.time - ActionInterval) ChangeGroundedStates(GroundedStates.Transition);
        else
        {
            xcurrent = Mathf.MoveTowards(xcurrent, PlayerInputHandler.Instance.MoveInput.x, TransitionSpeed * Time.deltaTime);
            zcurrent = Mathf.MoveTowards(zcurrent,PlayerInputHandler.Instance.MoveInput.y, TransitionSpeed * Time.deltaTime);
            
            anim.SetFloat("SpeedX", xcurrent);
            anim.SetFloat("SpeedZ", zcurrent);
            
            RotateTowardsMovement();
            
            controller.Move(moveDir * CurrentSpeed() * Time.deltaTime);
        }
    }
    void HandleGroundedWalkExit()
    {
        xcurrent = 0;
        zcurrent = 0;
        isWalking = false;
        anim.SetBool("Walk", isWalking);
    }
    
    void RotateTowardsMovement()
    {
        if (moveDir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            RotationSpeed * Time.deltaTime
        );
    }

    void HandleGroundedRunEnter()
    {
        isSprinting = true;
        anim.SetBool("Run", isSprinting);
    }
    void HandleGroundedRun()
    {
        if (ShouldEnterRun() && ActionEnd)
        {
            ActionEnd = false;
        }
        else if (!ShouldEnterRun() && !ActionEnd)
        {
            ActionEnd = true;
            ActionEndTime = Time.time;
        }
        
        if (!ShouldEnterRun() && ActionEndTime < Time.time - ActionInterval) ChangeGroundedStates(GroundedStates.Transition);
        else
        {
            RotateTowardsMovement();
            
            controller.Move(moveDir * CurrentSpeed() * Time.deltaTime);
        }
    }
    void HandleGroundedRunExit()
    {
        isSprinting = false;
        anim.SetBool("Run", isSprinting);
        
    }

    void HandleGroundedLand()
    {
        anim.SetTrigger("Land");
    }
    
#endregion

#region Ability States

    void AbilityCallInputs()
        {
            if (PlayerInputHandler.Instance.DodgeInput && CanDodge)
            {
                anim.applyRootMotion = true;
                currentAbilityStates = AbilityStates.Dodge;
            }
            else if (PlayerInputHandler.Instance.JumpInput && CanLeap)
            {
                anim.applyRootMotion = true;
                currentAbilityStates = AbilityStates.Leap;
            }
        }
    
    void HandleAbilityState()
    {
        switch (currentAbilityStates)
        {
            case AbilityStates.None:
                break;
            case AbilityStates.Dodge:
                HandleAbilityDodge();
                break;
            case AbilityStates.Leap:
                HandleAbilityLeap();
                break;
        }
    }

    void ChangeAbilityStates()
    {
        currentAbilityStates = AbilityStates.None;
    }

    void HandleAbilityDodge()
    {
        if (CanDodge)
        {
            CanDodge = false;
            anim.SetTrigger("Dodge");
        }
    }
    IEnumerator DodgeCooldown(float CooldownTime)
    {
        yield return new WaitForSeconds(CooldownTime);
        Debug.Log("Pinapple");
        CanDodge = true;
    }
    public void DodgeAbilityEnd()
    {
        anim.applyRootMotion = false;
        ChangeAbilityStates();
        StartCoroutine(DodgeCooldown(DodgeAbilityCooldown));
    }

    void HandleAbilityLeap()
    {
        if (CanLeap)
        {
            AnimationFinished = false;
            CanLeap = false;
            anim.SetTrigger("Leap");
        }
    }
    IEnumerator LeapCooldown(float CooldownTime)
    {
        yield return new WaitForSeconds(CooldownTime);
        CanLeap = true;
    }
    
    public void LeapAbilityEnd()
    {
        anim.applyRootMotion = false;
        ChangeAbilityStates();
        StartCoroutine(LeapCooldown(LeapAbilityCooldown));
        AnimationFinished = true;
        Debug.Log("Leap ended");
        
    }
    
#endregion

#region Combat

    public float attackConectionTime = 1f;
    public float counter = 0;
    public float hitcount = 0;
    public bool attackFinished = true;
    bool isattacking = false;
    private bool animcalled;
    private bool inCombat;
    
    public void AttackCallInputs()
    {
        if (attackFinished)
            counter -= Time.deltaTime;

        if (isattacking && !animcalled)
        {
            isattacking = false;
            animcalled = true;
            anim.SetTrigger("Attacking");
        }
        else if (!isattacking && animcalled)
        {
            animcalled = false;
        }
        
        if (PlayerInputHandler.Instance.AttackInput && attackFinished && hitcount == 0)
        {
            anim.applyRootMotion = true;
            inCombat = true;
            isattacking = true;
            attackFinished = false;
            PlayerInputHandler.Instance.AttackOver();
            anim.SetTrigger("Attack");
            counter = attackConectionTime;
            hitcount++;
        }
        else if (PlayerInputHandler.Instance.AttackInput && counter > 0 && hitcount == 1 && attackFinished)
        {
            isattacking = true;
            attackFinished = false;
            PlayerInputHandler.Instance.AttackOver();
            anim.SetTrigger("Attack 2");
            hitcount++;
        }
        else if (PlayerInputHandler.Instance.AttackInput && counter > 0 && hitcount == 2 && attackFinished)
        {
            isattacking = true;
            attackFinished = false;
            PlayerInputHandler.Instance.AttackOver();
            anim.SetTrigger("Attack 3");
            hitcount++;
        }
        else if (PlayerInputHandler.Instance.AttackInput && counter > 0 && hitcount == 3 && attackFinished)
        {
            isattacking = true;
            attackFinished = false;
            PlayerInputHandler.Instance.AttackOver();
            anim.SetTrigger("Attack 4");
            hitcount = 0;
        }
        else if (counter <= 0 )
        {
            anim.applyRootMotion = false;
            PlayerInputHandler.Instance.AttackOver();
            hitcount = 0;
            isattacking =  false;
            inCombat = false;
        }
    }

    public void AttackFinished()
    {
        attackFinished = true;
        counter = attackConectionTime;
    }

#endregion
}
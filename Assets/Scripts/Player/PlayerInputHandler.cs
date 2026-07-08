using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance;
    public PlayerInputMap Inputs;
    public Vector2 MoveInput;
    public bool SprintInput;
    public bool JumpInput;
    public bool DodgeInput;
    public bool grapple;

    public bool AttackInput;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Inputs = new PlayerInputMap();
        
    }
    void OnEnable()
    {
        Inputs.Enable();
    }

    void OnDisable()
    {
        Inputs.Disable();
    }
        
    void Update()
    {
        Inputs.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        Inputs.Player.Move.canceled += ctx => MoveInput = Vector2.zero; 

        Inputs.Player.Sprint.performed += ctx => SprintInput = true;
        Inputs.Player.Sprint.canceled += ctx => SprintInput = false;

        Inputs.Player.Jump.performed += ctx => JumpInput = true;
        Inputs.Player.Jump.canceled += ctx => JumpInput = false;
        
        Inputs.Player.Dodge.performed += ctx => DodgeInput = true;
        Inputs.Player.Dodge.canceled += ctx => DodgeInput = false;
        
        Inputs.Player.Grapple.performed += ctx => grapple = true;
        Inputs.Player.Grapple.canceled += ctx => grapple = false;
        
        Inputs.Player.Attack.performed += ctx => AttackInput = true;
        Inputs.Player.Attack.canceled += ctx => AttackInput = false;
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 12f;

    [Header("Refs")]
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform cameraTransform;
    
    [Header("Interact")]
    [SerializeField] KeyCode interactKey = KeyCode.E;
    [SerializeField] InteractionSensor interactionSensor;

    PlayerStateMachine stateMachine;
    PlayerIdleState idleState;
    PlayerMoveState moveState;

    Vector2 moveInput;
    Vector3 moveDirection;

    public Rigidbody PlayerRigidbody => playerRigidbody;
    public float MoveSpeed => moveSpeed;
    public float RotationSpeed => rotationSpeed;
    public Vector3 MoveDirection => moveDirection;
    public bool HasMoveInput => moveInput.sqrMagnitude > 0.01f;

    void Awake()
    {
        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
        
        if (interactionSensor == null)
            interactionSensor = GetComponent<InteractionSensor>();

        if (interactionSensor != null)
            interactionSensor.TargetChanged += OnTargetChanged;

        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);

        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        ReadMoveInput();
        BuildMoveDirection();
        
        TryInteractInput();

        stateMachine.Tick();
    }

    void FixedUpdate()
    {
        stateMachine.FixedTick();
    }

    void ReadMoveInput()
    {
        // Legacy Input (기본)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y);
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
    }
    
    void TryInteractInput()
    {
        if (interactionSensor == null)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            bool hasInteracted = interactionSensor.TryInteract(this);
            if (hasInteracted)
                Debug.Log("Interact Success");
        }
    }

    void BuildMoveDirection()
    {
        if (!HasMoveInput)
        {
            moveDirection = Vector3.zero;
            return;
        }

        if (cameraTransform == null)
        {
            moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            return;
        }

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 worldMove = (cameraRight * moveInput.x) + (cameraForward * moveInput.y);
        moveDirection = worldMove.normalized;
    }

    public void ChangeToIdle()
    {
        stateMachine.ChangeState(idleState);
    }

    public void ChangeToMove()
    {
        stateMachine.ChangeState(moveState);
    }
    
    void OnTargetChanged(IInteractable target)
    {
        if (target == null)
            Debug.Log("Interact Target: None");
        else
            Debug.Log($"Interact Target: {target.PromptText}");
    }
}

using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("References")]
    private InputSystem_Actions ctrl;

    [SerializeField] private CharacterController controller;
    [SerializeField] private CinemachineOrbitalFollow cinemachineOrbitalFollow;
    [SerializeField] private AbilityWheel abilityWheel;

    [Header("Movement")]

    [Tooltip("The speed which the player can move in units per second")] 
    [SerializeField, Min(0.0f)] private float movementSpeed;

    [Tooltip("The rate at which the player turns to match the camera in degrees per second")]
    [SerializeField, Min(0.0f)] private float turnRate;

    [Tooltip("The number of units that the player can jump")] 
    [SerializeField, Min(0.0f)] private float jumpHeight;
    private float jumpSpeed;

    [Tooltip("The acceleration due to gravity that the player experiences")] 
    [SerializeField, Min(0.0f)] private float gravity;

    [Tooltip("The maximum velocity by which the player can fall")] 
    [SerializeField, Min(0.0f)] private float terminalVelocity;

    public Vector3 velocity;
    private bool onGround;
    
    private const float GROUND_CHECK_DISTANCE = 0.1f;

    [Header("Abilities")]

    [SerializeField] private GameObject platformPrefab;
    private bool hasSand = false;
    private Sand currentDiggableSand = null;

    [ContextMenu("Recompute Constants")]
    private void InitDerivedConsts()
    {
        jumpSpeed = Mathf.Sqrt(2.0f * jumpHeight * gravity); // From kinematics
    }

    private void Awake()
    {
        ctrl = new();

        velocity = Vector3.zero;
        onGround = false;

        InitDerivedConsts();
    }

    private void OnEnable()
    {
        ctrl.Enable();
        ctrl.Player.Jump.performed += Jump;
        ctrl.Player.Dig.performed += Dig;
        ctrl.Player.Place.performed += Place;
        ctrl.Player.AbilityWheel.performed += OpenRecipeWheel;
        ctrl.Player.AbilityWheel.canceled += CloseRecipeWheel;
    }

    private void OnDisable()
    {
        ctrl.Player.Jump.performed -= Jump;
        ctrl.Player.Dig.performed -= Dig;
        ctrl.Player.Place.performed -= Place;
        ctrl.Player.AbilityWheel.performed -= OpenRecipeWheel;
        ctrl.Player.AbilityWheel.canceled -= CloseRecipeWheel;
        ctrl.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
    }

    private void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        Vector2 moveInput = ctrl.Player.Move.ReadValue<Vector2>();
        onGround = IsGrounded();

        float camYaw = cinemachineOrbitalFollow.HorizontalAxis.Value;
        Vector3 camFwd = new(Mathf.Sin(camYaw * Mathf.Deg2Rad), 0.0f, Mathf.Cos(camYaw * Mathf.Deg2Rad));
        Vector3 camRight = Vector3.Cross(Vector3.up, camFwd);
        Vector3 targetMoveDir = camFwd * moveInput.y + camRight * moveInput.x;
        float moveMagnitude = Mathf.Clamp01(moveInput.magnitude);

        if (moveMagnitude != 0.0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetMoveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, turnRate * fdt);
        }

        Vector3 movement = moveMagnitude * movementSpeed * fdt * transform.forward;
        // Fall
        if (onGround)
        {
            velocity.y = Mathf.Max(0.0f, velocity.y);
        }
        else
        {
            velocity.y = Mathf.Max(velocity.y - gravity * fdt, -terminalVelocity);
        }
        movement += velocity.y * fdt * Vector3.up;
        
        controller.Move(movement);

        velocity.x = controller.velocity.x;
        velocity.z = controller.velocity.z;
    }

    private bool IsGrounded()
    {
        return Physics.SphereCast(transform.position, controller.radius, Vector3.down, out RaycastHit hitInfo, GROUND_CHECK_DISTANCE);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sand") && other.TryGetComponent(out Sand sand))
        {
            currentDiggableSand = sand;
            currentDiggableSand.EnableHighlight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sand") && other.TryGetComponent(out Sand sand) && sand == currentDiggableSand)
        {
            currentDiggableSand.DisableHighlight();
            currentDiggableSand = null;
        }
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (!onGround) return;

        velocity.y = jumpSpeed;
    }

    private void Place(InputAction.CallbackContext ctx)
    {

    }

    private void Dig(InputAction.CallbackContext ctx)
    {
        if (currentDiggableSand != null && currentDiggableSand.isFull)
        {
            currentDiggableSand.Dig();
            currentDiggableSand = null;
            hasSand = true;
        }
    }

    private void OpenRecipeWheel(InputAction.CallbackContext ctx)
    {
        abilityWheel.Open();
    }

    private void CloseRecipeWheel(InputAction.CallbackContext ctx)
    {
        abilityWheel.Close();
    }
} 

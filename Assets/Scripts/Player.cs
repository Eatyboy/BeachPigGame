using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("References")]
    private InputSystem_Actions ctrl;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider col;
    [SerializeField] private CinemachineOrbitalFollow cinemachineOrbitalFollow;

    [Header("Parameters")]

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

    private bool onGround = false;
    public Vector3 velocity;

    [SerializeField] private float GROUND_CHECK_DISTANCE = 0.1f;
    [SerializeField] private float AUTO_ROT_THRESHOLD = 10f;

    [ContextMenu("Recompute Constants")]
    private void InitDerivedConsts()
    {
        jumpSpeed = Mathf.Sqrt(2.0f * jumpHeight * gravity); // From kinematics
    }

    private void Awake()
    {
        ctrl = new();

        InitDerivedConsts();
    }

    private void OnEnable()
    {
        ctrl.Enable();
        ctrl.Player.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        ctrl.Player.Jump.performed -= Jump;
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

        CheckGrounded();
        // Fall
        if (!onGround)
        {
            float newYVel = Mathf.Max(rb.linearVelocity.y - gravity * fdt, -terminalVelocity);
            rb.linearVelocity = new(rb.linearVelocity.x, newYVel, rb.linearVelocity.z);
        }

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

        Vector3 moveVel = moveMagnitude * movementSpeed * transform.forward;

        rb.linearVelocity = new(moveVel.x, rb.linearVelocity.y, moveVel.z);
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (!onGround) return;

        rb.linearVelocity = new(rb.linearVelocity.x, jumpSpeed, rb.linearVelocity.z);
    }

    private void CheckGrounded()
    {
        Vector3 origin = transform.TransformPoint(col.center) 
            + (col.radius - 0.01f) * Vector3.down;
        onGround = Physics.Raycast(origin, Vector3.down, out RaycastHit hit, GROUND_CHECK_DISTANCE);
    }
} 
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class MyFirstPersonController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 6.0f;
    public float rotationSpeed = 1.0f;
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;

    [Header("Camera Settings")]
    public GameObject cameraTarget;
    public float topClamp = 70.0f;
    public float bottomClamp = -30.0f;
    public float cameraAngleOverride = 0.0f;

    [Header("Jump Settings")]
    public float jumpTimeout = 0.1f;
    public float fallTimeout = 0.15f;

    private CharacterController controller;
    private MyPlayerInput input;
    private float speed;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    private float cinemachineTargetPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<MyPlayerInput>();
    }

    private void Start()
    {
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
    }

    private void Update()
    {
        ApplyJumpAndGravity();
        HandleMovement();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    private void HandleMovement()
    {
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y);

        if (inputDirection == Vector3.zero) speed = 0;
        else speed = input.sprint ? sprintSpeed : moveSpeed;

        inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
        inputDirection.Normalize();

        Vector3 velocity = inputDirection * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void RotateCamera()
    {
        if (input.look.sqrMagnitude >= 0.01f)
        {
            float mouseX = input.look.x * rotationSpeed;
            float mouseY = input.look.y * rotationSpeed;

            cinemachineTargetPitch -= mouseY;
            cinemachineTargetPitch = Mathf.Clamp(cinemachineTargetPitch, bottomClamp, topClamp);

            rotationVelocity = mouseX;

            transform.Rotate(Vector3.up * rotationVelocity);
            cameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, 0.0f, 0.0f);
        }
    }

    private void ApplyJumpAndGravity()
    {
        if (controller.isGrounded)
        {
            fallTimeoutDelta = fallTimeout;

            if (verticalVelocity < 0.0f)
                verticalVelocity = -2f;

            if (input.jump && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (jumpTimeoutDelta >= 0.0f)
                jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;

            input.jump = false;
        }

        if (verticalVelocity < terminalVelocity)
            verticalVelocity += gravity * Time.deltaTime;
    }
}

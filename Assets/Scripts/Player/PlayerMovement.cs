using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float minPitch = -70f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Vector3 moveInput;

    private float yaw;
    private float pitch;

    private float standingHeight;
    private Vector3 standingCenter;
    private Vector3 crouchCenter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        yaw = transform.eulerAngles.y;

        if (cameraTransform != null)
        {
            pitch = cameraTransform.localEulerAngles.x;
            if (pitch > 180f) pitch -= 360f;
        }

        standingHeight = capsuleCollider.height;
        standingCenter = capsuleCollider.center;

        crouchCenter = standingCenter;
        crouchCenter.y -= (standingHeight - crouchHeight) * 0.5f;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(targetRotation);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void HandleMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = (transform.forward * v + transform.right * h).normalized;
    }

    private void HandleMovement()
    {
        Vector3 targetPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = crouchCenter;
        }
        else
        {
            capsuleCollider.height = standingHeight;
            capsuleCollider.center = standingCenter;
        }
    }

    private bool IsGrounded()
    {
        float rayLength = (capsuleCollider.height * 0.5f) + groundCheckDistance;
        return Physics.Raycast(transform.position, Vector3.down, rayLength);
    }
}
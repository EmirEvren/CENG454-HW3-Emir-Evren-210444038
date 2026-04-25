using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float crouchMoveSpeed = 4f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivityX = 3f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 25f;
    [SerializeField] private float startPitch = 0f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6.5f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Crouch")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float standingCameraY = 1.6f;
    [SerializeField] private float crouchCameraY = 1.1f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private float yaw;
    private float pitch;

    private float inputX;
    private float inputZ;
    private bool jumpQueued;
    private bool isCrouching;

    private Vector3 standingCenter;
    private Vector3 crouchCenter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        yaw = transform.eulerAngles.y;
        pitch = startPitch;

        capsuleCollider.height = standingHeight;
        standingCenter = capsuleCollider.center;

        crouchCenter = standingCenter;
        crouchCenter.y -= (standingHeight - crouchHeight) * 0.5f;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraPivot != null)
        {
            Vector3 camPos = cameraPivot.localPosition;
            camPos.y = standingCameraY;
            cameraPivot.localPosition = camPos;
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void Update()
    {
        HandleMouseLook();
        HandleInput();
        HandleCrouch();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        yaw += mouseX * mouseSensitivityX;
        pitch -= mouseY * mouseSensitivityY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void HandleInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            jumpQueued = true;

        isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    private void HandleMovement()
    {
        float speed = isCrouching ? crouchMoveSpeed : moveSpeed;

        Vector3 moveDir = (transform.right * inputX + transform.forward * inputZ).normalized;
        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveDir.x * speed;
        velocity.z = moveDir.z * speed;

        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (!jumpQueued) return;
        jumpQueued = false;

        if (!IsGrounded()) return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleCrouch()
    {
        if (isCrouching)
        {
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = crouchCenter;

            if (cameraPivot != null)
            {
                Vector3 camPos = cameraPivot.localPosition;
                camPos.y = crouchCameraY;
                cameraPivot.localPosition = camPos;
            }
        }
        else
        {
            capsuleCollider.height = standingHeight;
            capsuleCollider.center = standingCenter;

            if (cameraPivot != null)
            {
                Vector3 camPos = cameraPivot.localPosition;
                camPos.y = standingCameraY;
                cameraPivot.localPosition = camPos;
            }
        }
    }

    private bool IsGrounded()
    {
        float rayLength = (capsuleCollider.height * 0.5f) + groundCheckDistance;
        return Physics.Raycast(transform.position, Vector3.down, rayLength);
    }
}
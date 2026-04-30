using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float crouchMoveSpeed = 2.5f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivityX = 3f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 25f;
    [SerializeField] private float startPitch = 0f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6.5f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundCheckRadiusMultiplier = 0.9f;
    [SerializeField] private float groundCheckOffset = 0.05f;

    [Header("Crouch")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float standingCameraY = 1.6f;
    [SerializeField] private float crouchCameraY = 1.1f;

    [Header("Footstep Audio")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip crouchWalkSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.75f;

    [Header("Footstep Timing")]
    [SerializeField] private float walkStepInterval = 0.45f;
    [SerializeField] private float runStepInterval = 0.28f;
    [SerializeField] private float crouchStepInterval = 0.65f;
    [SerializeField] private float minMoveSpeedForFootstep = 0.15f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private AudioSource footstepAudioSource;

    private float yaw;
    private float pitch;

    private float inputX;
    private float inputZ;

    private bool jumpQueued;
    private bool wantsToCrouch;
    private bool isCrouching;
    private bool isRunning;
    private bool isAiming;
    private bool isGrounded;

    private Vector3 standingCenter;
    private Vector3 crouchCenter;

    private float footstepTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        rb.constraints |= RigidbodyConstraints.FreezeRotationX;
        rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        SetupFootstepAudio();

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

        ApplyCameraHeight(standingCameraY);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Update()
    {
        HandleMouseLook();
        HandleInput();
        HandleFootsteps();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();

        rb.angularVelocity = Vector3.zero;

        ApplyBodyRotation();
        HandleCrouchPhysics();
        HandleMovement();
        HandleJump();
    }

    private void SetupFootstepAudio()
    {
        footstepAudioSource = GetComponent<AudioSource>();

        if (footstepAudioSource == null)
            footstepAudioSource = gameObject.AddComponent<AudioSource>();

        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.loop = false;
        footstepAudioSource.spatialBlend = 0f;
        footstepAudioSource.volume = footstepVolume;

        if (sfxMixerGroup != null)
            footstepAudioSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    private void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        yaw += mouseX * mouseSensitivityX;
        pitch -= mouseY * mouseSensitivityY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void ApplyBodyRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    private void HandleInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            jumpQueued = true;

        wantsToCrouch = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        isAiming = Input.GetMouseButton(1);
        isRunning = Input.GetKey(KeyCode.LeftShift) && inputZ > 0.1f && !isCrouching && !wantsToCrouch;
    }

    private void HandleMovement()
    {
        float speed = walkSpeed;

        if (isCrouching)
            speed = crouchMoveSpeed;
        else if (isRunning)
            speed = runSpeed;

        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 inputDirection = new Vector3(inputX, 0f, inputZ);
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        Vector3 moveDirection = yawRotation * inputDirection;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDirection.x * speed;
        velocity.z = moveDirection.z * speed;

        rb.linearVelocity = velocity;
    }

    private void HandleFootsteps()
    {
        if (!isGrounded)
        {
            footstepTimer = 0f;
            return;
        }

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        bool isMoving = horizontalVelocity.magnitude > minMoveSpeedForFootstep;

        if (!isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer > 0f)
            return;

        PlayFootstepSound();
        footstepTimer = GetCurrentFootstepInterval();
    }

    private void PlayFootstepSound()
    {
        if (footstepAudioSource == null)
            return;

        AudioClip selectedClip = GetCurrentFootstepClip();

        if (selectedClip == null)
            return;

        footstepAudioSource.PlayOneShot(selectedClip, footstepVolume);
    }

    private AudioClip GetCurrentFootstepClip()
    {
        if (isCrouching)
        {
            if (crouchWalkSound != null)
                return crouchWalkSound;

            return walkSound;
        }

        if (isRunning)
        {
            if (runSound != null)
                return runSound;

            return walkSound;
        }

        return walkSound;
    }

    private float GetCurrentFootstepInterval()
    {
        if (isCrouching)
            return crouchStepInterval;

        if (isRunning)
            return runStepInterval;

        return walkStepInterval;
    }

    private void HandleJump()
    {
        if (!jumpQueued)
            return;

        jumpQueued = false;

        if (!isGrounded)
            return;

        if (isCrouching)
        {
            if (!CanStandUp())
                return;

            SetCrouch(false);
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        footstepTimer = 0f;

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    private void HandleCrouchPhysics()
    {
        if (wantsToCrouch)
        {
            if (isGrounded)
                SetCrouch(true);
        }
        else
        {
            if (CanStandUp())
                SetCrouch(false);
        }
    }

    private void SetCrouch(bool value)
    {
        if (isCrouching == value)
            return;

        isCrouching = value;

        if (isCrouching)
        {
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = crouchCenter;

            ApplyCameraHeight(crouchCameraY);
        }
        else
        {
            capsuleCollider.height = standingHeight;
            capsuleCollider.center = standingCenter;

            ApplyCameraHeight(standingCameraY);
        }
    }

    private void ApplyCameraHeight(float height)
    {
        if (cameraPivot == null)
            return;

        Vector3 camPos = cameraPivot.localPosition;
        camPos.y = height;
        cameraPivot.localPosition = camPos;
    }

    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        float moveAmount = horizontalVelocity.magnitude;

        animator.SetFloat("MoveAmount", moveAmount);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsAiming", isAiming);
    }

    private bool CheckGrounded()
    {
        Bounds bounds = capsuleCollider.bounds;
        float radius = Mathf.Max(0.05f, capsuleCollider.radius * groundCheckRadiusMultiplier);

        Vector3 checkPos = new Vector3(
            bounds.center.x,
            bounds.min.y + groundCheckOffset,
            bounds.center.z
        );

        return Physics.CheckSphere(checkPos, radius, groundMask, QueryTriggerInteraction.Ignore);
    }

    private bool CanStandUp()
    {
        if (!isCrouching)
            return true;

        Bounds bounds = capsuleCollider.bounds;
        float radius = Mathf.Max(0.05f, capsuleCollider.radius * 0.9f);
        float extraHeightNeeded = standingHeight - crouchHeight;

        Vector3 origin = new Vector3(
            bounds.center.x,
            bounds.max.y - radius,
            bounds.center.z
        );

        return !Physics.SphereCast(
            origin,
            radius,
            Vector3.up,
            out _,
            extraHeightNeeded + 0.05f,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }
}
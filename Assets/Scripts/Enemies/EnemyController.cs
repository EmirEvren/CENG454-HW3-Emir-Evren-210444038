using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Ranged
    }

    [Header("Type")]
    [SerializeField] private EnemyType enemyType = EnemyType.Melee;

    [Header("Targets")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform chestTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float playerDetectionRange = 6f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Attack")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Melee Settings")]
    [SerializeField] private float meleeAttackRange = 1.5f;

    [Header("Ranged Settings")]
    [SerializeField] private float fixedRangedAttackRange = 9f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private Transform currentTarget;
    private bool lockedOnChest;
    private float attackRange;
    private float lastAttackTime;

    public bool IsRanged => enemyType == EnemyType.Ranged;
    public float AttackRange => attackRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        SetupEnemyType();
    }

    private void Start()
    {
        InitializeTargets();
    }

    private void Update()
    {
        if (playerTarget == null || chestTarget == null)
            return;

        SelectTarget();
    }

    private void FixedUpdate()
    {
        MoveAndAttack();
    }

    public void InitializeTargets()
    {
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }

        if (chestTarget == null)
        {
            ChestHealth chest = FindFirstObjectByType<ChestHealth>();
            if (chest != null)
                chestTarget = chest.transform;
        }
    }

    public void ConfigureDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void SetupEnemyType()
    {
        if (enemyType == EnemyType.Melee)
            attackRange = meleeAttackRange;
        else
            attackRange = fixedRangedAttackRange;
    }

    private void SelectTarget()
    {
        if (lockedOnChest)
        {
            currentTarget = chestTarget;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= playerDetectionRange)
            currentTarget = playerTarget;
        else
            currentTarget = chestTarget;

        if (currentTarget == chestTarget)
        {
            float distanceToChest = Vector3.Distance(transform.position, chestTarget.position);

            if (distanceToChest <= attackRange)
                lockedOnChest = true;
        }
    }

    private void MoveAndAttack()
    {
        if (currentTarget == null)
        {
            StopHorizontalMovement();
            return;
        }

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            Quaternion newRotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(newRotation);
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
        {
            Vector3 moveDir = direction.normalized;
            Vector3 velocity = rb.linearVelocity;
            velocity.x = moveDir.x * moveSpeed;
            velocity.z = moveDir.z * moveSpeed;
            rb.linearVelocity = velocity;
        }
        else
        {
            StopHorizontalMovement();
            TryAttack();
        }
    }

    private void StopHorizontalMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.linearVelocity = velocity;
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        if (currentTarget == null)
            return;

        IDamageable damageable = currentTarget.GetComponent<IDamageable>();

        if (damageable == null)
            damageable = currentTarget.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            damageable.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}
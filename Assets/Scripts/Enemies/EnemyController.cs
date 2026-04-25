using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private enum EnemyType
    {
        Melee,
        Ranged
    }

    [Header("Targets")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform chestTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float playerDetectionRange = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Melee Settings")]
    [SerializeField] private float meleeAttackRange = 1.5f;

    [Header("Ranged Settings")]
    [SerializeField] private float minRangedAttackRange = 7f;
    [SerializeField] private float maxRangedAttackRange = 12f;

    [Header("Spawn Chance")]
    [SerializeField, Range(0f, 1f)] private float meleeChance = 0.6f;

    private Transform currentTarget;
    private bool lockedOnChest;
    private float attackRange;
    private float lastAttackTime;
    private EnemyType enemyType;

    public bool IsRanged => enemyType == EnemyType.Ranged;
    public float AttackRange => attackRange;

    private void Awake()
    {
        DecideEnemyType();
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

    private void DecideEnemyType()
    {
        float randomValue = Random.value;

        if (randomValue <= meleeChance)
        {
            enemyType = EnemyType.Melee;
            attackRange = meleeAttackRange;
        }
        else
        {
            enemyType = EnemyType.Ranged;
            attackRange = Random.Range(minRangedAttackRange, maxRangedAttackRange);
        }
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
            return;

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            TryAttack();
        }
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
            damageable.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}
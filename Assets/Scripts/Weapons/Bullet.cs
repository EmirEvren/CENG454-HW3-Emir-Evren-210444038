using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;

    [Header("Damage Modifier Chain")]
    [SerializeField] private bool requireColorMatch = true;
    [SerializeField] private bool useBonusDamageModifier = false;
    [SerializeField, Min(0)] private int bonusDamage = 0;

    private int damage;
    private AmmoColor ammoColor = AmmoColor.None;
    private float timer;
    private Vector3 moveDirection;

    private BulletPool ownerPool;
    private Transform bulletRoot;
    private bool isReleased;

    private IWeaponDamageModifier damageModifier;

    public AmmoColor AmmoColor => ammoColor;
    public int Damage => damage;
    public GameObject RootObject => bulletRoot != null ? bulletRoot.gameObject : gameObject;

    private void Awake()
    {
        bulletRoot = transform.parent != null ? transform.parent : transform;
        BuildDamageModifierChain();
    }

    private void OnEnable()
    {
        timer = lifeTime;
        isReleased = false;
    }

    private void Update()
    {
        if (isReleased)
            return;

        RootObject.transform.position += moveDirection * speed * Time.deltaTime;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    public void AssignPool(BulletPool pool)
    {
        ownerPool = pool;
    }

    public void Initialize(int bulletDamage, AmmoColor color, Vector3 direction)
    {
        damage = bulletDamage;
        ammoColor = color;
        moveDirection = direction.normalized;
        timer = lifeTime;
        isReleased = false;

        BuildDamageModifierChain();

        if (moveDirection.sqrMagnitude > 0.001f)
            RootObject.transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    public void OnSpawnedFromPool()
    {
        timer = lifeTime;
        isReleased = false;
    }

    public void OnReturnedToPool()
    {
        damage = 0;
        ammoColor = AmmoColor.None;
        timer = lifeTime;
        moveDirection = Vector3.zero;
        isReleased = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReleased)
            return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            ApplyDamageToEnemy(enemy);
            ReturnToPool();
            return;
        }

        if (other.CompareTag("Wall"))
        {
            ReturnToPool();
        }
    }

    private void ApplyDamageToEnemy(EnemyHealth enemy)
    {
        if (enemy == null)
            return;

        if (damageModifier == null)
            BuildDamageModifierChain();

        int finalDamage = damageModifier.ModifyDamage(
            damage,
            ammoColor,
            enemy.EnemyColor
        );

        if (finalDamage <= 0)
            return;

        enemy.TakeDamage(finalDamage);
    }

    private void BuildDamageModifierChain()
    {
        IWeaponDamageModifier modifier = new BaseWeaponDamageModifier();

        if (requireColorMatch)
            modifier = new ColorMatchDamageModifier(modifier);

        if (useBonusDamageModifier && bonusDamage > 0)
            modifier = new BonusDamageModifier(modifier, bonusDamage);

        damageModifier = modifier;
    }

    private void ReturnToPool()
    {
        if (isReleased)
            return;

        if (ownerPool != null)
        {
            ownerPool.ReturnBullet(this);
        }
        else
        {
            Destroy(RootObject);
        }
    }
}
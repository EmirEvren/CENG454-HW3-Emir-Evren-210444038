using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;

    private int damage;
    private AmmoColor ammoColor = AmmoColor.None;
    private float timer;
    private Vector3 moveDirection;

    private BulletPool ownerPool;
    private Transform bulletRoot;
    private bool isReleased;

    public AmmoColor AmmoColor => ammoColor;
    public int Damage => damage;
    public GameObject RootObject => bulletRoot != null ? bulletRoot.gameObject : gameObject;

    private void Awake()
    {
        bulletRoot = transform.parent != null ? transform.parent : transform;
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
            if (enemy.EnemyColor == ammoColor)
            {
                enemy.TakeDamage(damage);
            }

            ReturnToPool();
            return;
        }

        if (other.CompareTag("Wall"))
        {
            ReturnToPool();
        }
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
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;

    private int damage;
    private AmmoColor ammoColor;
    private float timer;

    public AmmoColor AmmoColor => ammoColor;
    public int Damage => damage;

    private Transform bulletRoot;

    private void Awake()
    {
        bulletRoot = transform.parent != null ? transform.parent : transform;
    }

    private void OnEnable()
    {
        timer = lifeTime;
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(bulletRoot.gameObject);
        }
    }

    public void Initialize(int bulletDamage, AmmoColor color)
    {
        damage = bulletDamage;
        ammoColor = color;
        timer = lifeTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            if (enemy.EnemyColor == ammoColor)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(bulletRoot.gameObject);
            return;
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(bulletRoot.gameObject);
        }
    }
}
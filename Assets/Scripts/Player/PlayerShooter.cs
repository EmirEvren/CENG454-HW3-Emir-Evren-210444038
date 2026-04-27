using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;

    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject redBulletPrefab;
    [SerializeField] private GameObject yellowBulletPrefab;
    [SerializeField] private GameObject greenBulletPrefab;
    [SerializeField] private GameObject blueBulletPrefab;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float maxAimDistance = 200f;
    [SerializeField] private LayerMask aimMask = ~0;

    private float fireTimer;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (firePoint == null)
        {
            GameObject firePointObj = GameObject.FindGameObjectWithTag("firePoint");
            if (firePointObj != null)
                firePoint = firePointObj.transform;
        }
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        HandleColorSelection();
        HandleShooting();
    }

    private void HandleColorSelection()
    {
        if (ammoInventory == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ammoInventory.SetCurrentColor(AmmoColor.Red);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ammoInventory.SetCurrentColor(AmmoColor.Yellow);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ammoInventory.SetCurrentColor(AmmoColor.Green);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            ammoInventory.SetCurrentColor(AmmoColor.Blue);

        if (Input.GetKeyDown(KeyCode.Q))
            ammoInventory.ClearSelection();
    }

    private void HandleShooting()
    {
        if (!Input.GetMouseButton(0))
            return;

        if (fireTimer > 0f)
            return;

        if (ammoInventory == null || mainCamera == null || firePoint == null)
            return;

        if (ammoInventory.CurrentAmmoColor == AmmoColor.None)
            return;

        bool consumed = ammoInventory.TryConsumeCurrentAmmo(1);
        if (!consumed)
            return;

        GameObject selectedBulletPrefab = GetBulletPrefab(ammoInventory.CurrentAmmoColor);
        if (selectedBulletPrefab == null)
            return;

        fireTimer = fireRate;

        if (animator != null)
        {
            animator.ResetTrigger("Shoot");
            animator.SetTrigger("Shoot");
        }

        Vector3 aimPoint = GetAimPoint();
        Vector3 shootDirection = (aimPoint - firePoint.position).normalized;

        GameObject bulletObj = Instantiate(
            selectedBulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet == null)
            bullet = bulletObj.GetComponentInChildren<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(
                bulletDamage,
                ammoInventory.CurrentAmmoColor,
                shootDirection
            );
        }
    }

    private Vector3 GetAimPoint()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        return ray.origin + ray.direction * maxAimDistance;
    }

    private GameObject GetBulletPrefab(AmmoColor color)
    {
        switch (color)
        {
            case AmmoColor.Red:
                return redBulletPrefab;
            case AmmoColor.Yellow:
                return yellowBulletPrefab;
            case AmmoColor.Green:
                return greenBulletPrefab;
            case AmmoColor.Blue:
                return blueBulletPrefab;
            default:
                return null;
        }
    }
}
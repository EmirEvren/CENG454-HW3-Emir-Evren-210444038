using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private Transform firePoint;

    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject redBulletPrefab;
    [SerializeField] private GameObject yellowBulletPrefab;
    [SerializeField] private GameObject greenBulletPrefab;
    [SerializeField] private GameObject blueBulletPrefab;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int bulletDamage = 1;

    private float fireTimer;

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        HandleColorSelection();
        HandleShooting();
    }

    private void HandleColorSelection()
    {
        if (ammoInventory == null) return;

        // 1 = Red
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ammoInventory.SetCurrentColor(AmmoColor.Red);

        // 2 = Yellow
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ammoInventory.SetCurrentColor(AmmoColor.Yellow);

        // 3 = Green
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ammoInventory.SetCurrentColor(AmmoColor.Green);

        // 4 = Blue
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ammoInventory.SetCurrentColor(AmmoColor.Blue);
    }

    private void HandleShooting()
    {
        if (!Input.GetMouseButton(0)) return;
        if (fireTimer > 0f) return;
        if (ammoInventory == null || firePoint == null) return;

        bool consumed = ammoInventory.TryConsumeCurrentAmmo(1);
        if (!consumed) return;

        GameObject selectedBulletPrefab = GetBulletPrefab(ammoInventory.CurrentAmmoColor);
        if (selectedBulletPrefab == null) return;

        fireTimer = fireRate;

        GameObject bulletObj = Instantiate(selectedBulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Initialize(bulletDamage, ammoInventory.CurrentAmmoColor);
        }
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
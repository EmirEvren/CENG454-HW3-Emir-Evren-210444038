using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private Camera mainCamera;

    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject redBulletPrefab;
    [SerializeField] private GameObject yellowBulletPrefab;
    [SerializeField] private GameObject greenBulletPrefab;
    [SerializeField] private GameObject blueBulletPrefab;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float bulletSpawnDistance = 0.5f;

    private float fireTimer;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        HandleColorSelection();
        HandleShooting();
    }

    private void HandleColorSelection()
    {
        if (ammoInventory == null) return;

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
        if (!Input.GetMouseButton(0)) return;
        if (fireTimer > 0f) return;
        if (ammoInventory == null || mainCamera == null) return;
        if (ammoInventory.CurrentAmmoColor == AmmoColor.None) return;

        bool consumed = ammoInventory.TryConsumeCurrentAmmo(1);
        if (!consumed) return;

        GameObject selectedBulletPrefab = GetBulletPrefab(ammoInventory.CurrentAmmoColor);
        if (selectedBulletPrefab == null) return;

        fireTimer = fireRate;

        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * bulletSpawnDistance;
        Quaternion spawnRotation = Quaternion.LookRotation(mainCamera.transform.forward);

        GameObject bulletObj = Instantiate(selectedBulletPrefab, spawnPosition, spawnRotation);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet == null)
            bullet = bulletObj.GetComponentInChildren<Bullet>();

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
using UnityEngine;
using UnityEngine.Audio;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;

    [Header("Pooling")]
    [SerializeField] private BulletPool bulletPool;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float maxAimDistance = 200f;
    [SerializeField] private LayerMask aimMask = ~0;

    [Header("Shooting Audio")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Range(0f, 1f)] private float shootSoundVolume = 1f;

    private AudioSource audioSource;
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

        if (bulletPool == null)
            bulletPool = FindFirstObjectByType<BulletPool>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = shootSoundVolume;

        if (sfxMixerGroup != null)
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
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

        if (ammoInventory == null || mainCamera == null || firePoint == null || bulletPool == null)
            return;

        AmmoColor selectedColor = ammoInventory.CurrentAmmoColor;

        if (selectedColor == AmmoColor.None)
            return;

        bool consumed = ammoInventory.TryConsumeCurrentAmmo(1);

        if (!consumed)
            return;

        Vector3 aimPoint = GetAimPoint();
        Vector3 shootDirection = (aimPoint - firePoint.position).normalized;

        Bullet bullet = bulletPool.GetBullet(
            selectedColor,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        if (bullet == null)
        {
            ammoInventory.AddAmmo(selectedColor, 1);
            return;
        }

        fireTimer = fireRate;

        if (animator != null)
        {
            animator.ResetTrigger("Shoot");
            animator.SetTrigger("Shoot");
        }

        bullet.Initialize(
            bulletDamage,
            selectedColor,
            shootDirection
        );

        PlayShootSound();
    }

    private void PlayShootSound()
    {
        if (audioSource == null || shootSound == null)
            return;

        audioSource.PlayOneShot(shootSound, shootSoundVolume);
    }

    private Vector3 GetAimPoint()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        return ray.origin + ray.direction * maxAimDistance;
    }
}
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private AmmoColor enemyColor;

    [SerializeField] private int maxHealth = 1;

    [Header("Optional Visual")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material blueMaterial;

    private int currentHealth;

    public AmmoColor EnemyColor => enemyColor;

    public event Action<EnemyHealth> OnEnemyDied;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ConfigureHealth(int newHealth)
    {
        maxHealth = newHealth;
        currentHealth = maxHealth;
    }

    public void ConfigureColor(AmmoColor newColor)
    {
        enemyColor = newColor;
        ApplyColorVisual();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            OnEnemyDied?.Invoke(this);
            Destroy(gameObject);
        }
    }

    private void ApplyColorVisual()
    {
        if (meshRenderer == null) return;

        switch (enemyColor)
        {
            case AmmoColor.Red:
                meshRenderer.material = redMaterial;
                break;
            case AmmoColor.Yellow:
                meshRenderer.material = yellowMaterial;
                break;
            case AmmoColor.Green:
                meshRenderer.material = greenMaterial;
                break;
            case AmmoColor.Blue:
                meshRenderer.material = blueMaterial;
                break;
        }
    }
}
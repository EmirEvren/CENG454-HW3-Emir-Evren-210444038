using System;
using UnityEngine;

public class ChestHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 500;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnChestDestroyed;

    private bool isDestroyed;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;
        if (amount <= 0) return;

        CurrentHealth -= amount;

        if (CurrentHealth < 0)
            CurrentHealth = 0;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            isDestroyed = true;
            OnChestDestroyed?.Invoke();
            Debug.Log("Chest destroyed. Game Over.");
        }
    }

    public void ResetHealth()
    {
        isDestroyed = false;
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
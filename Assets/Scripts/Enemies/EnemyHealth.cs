using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private AmmoColor enemyColor = AmmoColor.Red;
    [SerializeField] private int maxHealth = 1;

    private int currentHealth;

    public AmmoColor EnemyColor => enemyColor;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
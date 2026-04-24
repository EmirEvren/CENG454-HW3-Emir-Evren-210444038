using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestHealthUI : MonoBehaviour
{
    [SerializeField] private ChestHealth chestHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private void OnEnable()
    {
        if (chestHealth != null)
        {
            chestHealth.OnHealthChanged += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (chestHealth != null)
        {
            chestHealth.OnHealthChanged -= UpdateUI;
        }
    }

    private void Start()
    {
        if (chestHealth != null)
        {
            UpdateUI(chestHealth.CurrentHealth, chestHealth.MaxHealth);
        }
    }

    private void UpdateUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"Chest HP: {currentHealth} / {maxHealth}";
        }
    }
}
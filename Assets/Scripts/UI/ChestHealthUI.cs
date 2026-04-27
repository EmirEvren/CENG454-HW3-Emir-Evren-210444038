using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutine kullanabilmek için ekledik

public class ChestHealthUI : MonoBehaviour
{
    [SerializeField] private ChestHealth chestHealth;
    
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel; 
    [SerializeField] private Slider healthSlider; 
    [SerializeField] private TMP_Text healthText;

    [Header("Settings")]
    [SerializeField] private float hideDelay = 0.5f; // Kapanmadan önce kaç saniye bekleyecek

    private void OnEnable()
    {
        if (chestHealth != null)
            chestHealth.OnHealthChanged += UpdateUI;
    }

    private void OnDisable()
    {
        if (chestHealth != null)
            chestHealth.OnHealthChanged -= UpdateUI;
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
        // 1. ÖNCE HER ŞARTTA UI'I GÜNCELLE (Böylece 5'te takılı kalmaz, 0'ı görürüz)
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            // Eğer can 0'ın altına düşerse (eksi değerler), ekranda sadece 0 yazsın
            int displayHealth = Mathf.Max(0, currentHealth); 
            healthText.text = $"Chest: {displayHealth}/{maxHealth}";
        }

        // 2. EĞER CAN SIFIRLANDIYSA KAPANMA SÜRECİNİ BAŞLAT
        if (currentHealth <= 0)
        {
            StartCoroutine(HidePanelRoutine());
        }
        else
        {
            // Eğer can 0'dan büyükse ve panel kapalıysa geri aç
            if (uiPanel != null && !uiPanel.activeSelf)
            {
                uiPanel.SetActive(true);
            }
        }
    }

    // Gecikmeli kapatma işlemi
    private IEnumerator HidePanelRoutine()
    {
        // Belirlediğimiz süre kadar (örn: 0.5 saniye) bekle
        yield return new WaitForSeconds(hideDelay);

        // Süre bitince paneli gizle
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }
}
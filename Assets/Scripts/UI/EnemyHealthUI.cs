using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Transform healthBarContainer; // Horizontal Layout Group objesi
    [SerializeField] private GameObject healthSegmentPrefab; // 1 adet can barı (Image)

    [Header("Colors")]
    [SerializeField] private Color fullHealthColor = Color.red;
    [SerializeField] private Color emptyHealthColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Saydam gri

    private List<Image> healthSegments = new List<Image>();

    private void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        // Gerekirse yeni segmentler (barlar) üret (Stage ilerledikçe can artarsa diye)
        while (healthSegments.Count < maxHealth)
        {
            GameObject newSegment = Instantiate(healthSegmentPrefab, healthBarContainer);
            Image segmentImage = newSegment.GetComponent<Image>();
            healthSegments.Add(segmentImage);
        }

        // Barların renklerini güncel cana göre ayarla
        for (int i = 0; i < healthSegments.Count; i++)
        {
            if (i < maxHealth)
            {
                healthSegments[i].gameObject.SetActive(true);
                // Eğer index güncel candan küçükse içi dolu renk (kırmızı), değilse boş renk (gri)
                healthSegments[i].color = (i < currentHealth) ? fullHealthColor : emptyHealthColor;
            }
            else
            {
                // Ekstra barları gizle (Eğer max can düşerse diye önlem)
                healthSegments[i].gameObject.SetActive(false);
            }
        }
    }
}
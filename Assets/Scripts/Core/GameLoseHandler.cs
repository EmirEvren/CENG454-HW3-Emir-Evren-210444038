using System.Collections;
using UnityEngine;

public class GameLoseHandler : MonoBehaviour
{
    [SerializeField] private ChestHealth chestHealth;
    [SerializeField] private GameObject losePanel;

    [Header("Settings")]
    [Tooltip("UI kapandıktan sonra Lose ekranının gelmesi için geçecek süre.")]
    [SerializeField] private float loseDelay = 0.6f; // ChestHealthUI'da 0.5s beklemiştik, burada 0.6s idealdir.

    private void OnEnable()
    {
        if (chestHealth != null)
        {
            chestHealth.OnChestDestroyed += HandleLose;
        }
    }

    private void OnDisable()
    {
        if (chestHealth != null)
        {
            chestHealth.OnChestDestroyed -= HandleLose;
        }
    }

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);
    }

    private void HandleLose()
    {
        // Oyunu anında dondurmak yerine gecikme rutinini başlatıyoruz
        StartCoroutine(LoseRoutine());
    }

    private IEnumerator LoseRoutine()
    {
        // Sandığın patlama anını ve UI'ın kapanmasını (0.5 saniye) izlettir
        yield return new WaitForSeconds(loseDelay);

        Debug.Log("LOSE STATE TRIGGERED");

        if (losePanel != null)
            losePanel.SetActive(true);

        // Ekran geldikten SONRA oyunu tamamen durdur
        Time.timeScale = 0f;
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

public class GameLoseHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChestHealth chestHealth;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("UI")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text loseText;

    [Header("Lose Messages")]
    [SerializeField] private string playerDeadMessage = "PLAYER DEAD";
    [SerializeField] private string chestDestroyedMessage = "CHEST DESTROYED";
    [SerializeField] private string gameOverMessage = "GAME OVER";

    [Header("Settings")]
    [SerializeField] private float gameOverTextDelay = 3f;

    private bool loseTriggered;

    private void OnEnable()
    {
        if (chestHealth != null)
            chestHealth.OnChestDestroyed += HandleChestDestroyed;

        if (playerHealth != null)
            playerHealth.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        if (chestHealth != null)
            chestHealth.OnChestDestroyed -= HandleChestDestroyed;

        if (playerHealth != null)
            playerHealth.OnPlayerDied -= HandlePlayerDied;
    }

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);
    }

    private void HandlePlayerDied()
    {
        TriggerLose(playerDeadMessage);
    }

    private void HandleChestDestroyed()
    {
        TriggerLose(chestDestroyedMessage);
    }

    private void TriggerLose(string firstMessage)
    {
        if (loseTriggered)
            return;

        loseTriggered = true;
        StartCoroutine(LoseTextRoutine(firstMessage));
    }

    private IEnumerator LoseTextRoutine(string firstMessage)
    {
        Debug.Log($"LOSE STATE TRIGGERED: {firstMessage}");

        if (losePanel != null)
            losePanel.SetActive(true);

        if (loseText != null)
            loseText.text = firstMessage;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSeconds(gameOverTextDelay);

        if (loseText != null)
            loseText.text = gameOverMessage;

        Time.timeScale = 0f;
    }
}
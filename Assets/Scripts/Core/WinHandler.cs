using System.Collections;
using TMPro;
using UnityEngine;

public class WinHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StageManager stageManager;

    [Header("Center Message UI")]
    [SerializeField] private GameObject centerMessagePanel;
    [SerializeField] private TMP_Text centerMessageText;

    [Header("Win UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winText;

    [Header("Messages")]
    [SerializeField] private string lootTimeMessage = "LOOT TIME";
    [SerializeField] private string stageClearedMessage = "STAGE CLEARED";
    [SerializeField] private string winMessage = "YOU WIN";

    [Header("Settings")]
    [SerializeField] private float centerMessageDuration = 1.5f;
    [SerializeField] private bool pauseGameOnWin = true;

    private Coroutine messageRoutine;
    private bool winTriggered;

    private void OnEnable()
    {
        if (stageManager != null)
        {
            stageManager.OnLootPhaseStarted += HandleLootPhaseStarted;
            stageManager.OnStageCleared += HandleStageCleared;
            stageManager.OnAllStagesCompleted += HandleWin;
        }
    }

    private void OnDisable()
    {
        if (stageManager != null)
        {
            stageManager.OnLootPhaseStarted -= HandleLootPhaseStarted;
            stageManager.OnStageCleared -= HandleStageCleared;
            stageManager.OnAllStagesCompleted -= HandleWin;
        }
    }

    private void Start()
    {
        if (centerMessagePanel != null)
            centerMessagePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void HandleLootPhaseStarted(int stageNumber)
    {
        ShowTemporaryCenterMessage(lootTimeMessage);
    }

    private void HandleStageCleared(int stageNumber)
    {
        ShowTemporaryCenterMessage(stageClearedMessage);
    }

    private void HandleWin()
    {
        if (winTriggered)
            return;

        winTriggered = true;

        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
            messageRoutine = null;
        }

        if (centerMessagePanel != null && centerMessagePanel != winPanel)
            centerMessagePanel.SetActive(false);

        if (winText != null)
            winText.text = winMessage;

        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseGameOnWin)
            Time.timeScale = 0f;

        Debug.Log("WIN STATE TRIGGERED.");
    }

    private void ShowTemporaryCenterMessage(string message)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(TemporaryCenterMessageRoutine(message));
    }

    private IEnumerator TemporaryCenterMessageRoutine(string message)
    {
        if (centerMessageText != null)
            centerMessageText.text = message;

        if (centerMessagePanel != null)
            centerMessagePanel.SetActive(true);

        yield return new WaitForSeconds(centerMessageDuration);

        if (centerMessagePanel != null)
            centerMessagePanel.SetActive(false);

        messageRoutine = null;
    }
}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Audio")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Range(0f, 1f)] private float winSoundVolume = 1f;

    [Header("Settings")]
    [SerializeField] private float centerMessageDuration = 1.5f;
    [SerializeField] private bool pauseGameOnWin = true;
    [SerializeField] private float freezeDelayAfterWin = 2f;

    private AudioSource audioSource;
    private Coroutine messageRoutine;
    private Coroutine winRoutine;
    private bool winTriggered;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = winSoundVolume;

        if (sfxMixerGroup != null)
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
    }

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

        if (winRoutine != null)
            StopCoroutine(winRoutine);

        winRoutine = StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
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

        PlayWinSound();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("WIN STATE TRIGGERED. Waiting before freeze.");

        yield return new WaitForSecondsRealtime(freezeDelayAfterWin);

        if (pauseGameOnWin)
            Time.timeScale = 0f;

        Debug.Log("WIN STATE FREEZED.");
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

    private void PlayWinSound()
    {
        if (audioSource == null || winSound == null)
            return;

        audioSource.PlayOneShot(winSound, winSoundVolume);
    }
}
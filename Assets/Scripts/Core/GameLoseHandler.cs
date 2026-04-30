using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameLoseHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChestHealth chestHealth;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private MainMenuUIManager mainMenuUIManager;

    [Header("UI")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text loseText;

    [Header("Lose Messages")]
    [SerializeField] private string playerDeadMessage = "PLAYER DEAD";
    [SerializeField] private string chestDestroyedMessage = "CHEST DESTROYED";
    [SerializeField] private string gameOverMessage = "GAME OVER";

    [Header("Audio")]
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Range(0f, 1f)] private float loseSoundVolume = 1f;

    [Header("Settings")]
    [SerializeField] private float gameOverTextDelay = 3f;
    [SerializeField] private float returnToMainMenuDelay = 5f;

    private AudioSource audioSource;
    private bool loseTriggered;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = loseSoundVolume;

        if (sfxMixerGroup != null)
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
    }

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

        PlayLoseSound();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSecondsRealtime(gameOverTextDelay);

        if (loseText != null)
            loseText.text = gameOverMessage;

        float remainingDelay = returnToMainMenuDelay - gameOverTextDelay;

        if (remainingDelay > 0f)
            yield return new WaitForSecondsRealtime(remainingDelay);

        ReturnToMainMenu();
    }

    private void PlayLoseSound()
    {
        if (audioSource == null || loseSound == null)
            return;

        audioSource.PlayOneShot(loseSound, loseSoundVolume);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (mainMenuUIManager != null)
        {
            mainMenuUIManager.ReturnToMainMenuAndResetGame();
            return;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
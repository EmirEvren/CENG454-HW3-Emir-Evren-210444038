using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameLoseHandler : MonoBehaviour
{
    [SerializeField] private ChestHealth chestHealth;
<<<<<<< Updated upstream
=======
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private MainMenuUIManager mainMenuUIManager;

    [Header("UI")]
>>>>>>> Stashed changes
    [SerializeField] private GameObject losePanel;

    [Header("Audio")]
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Range(0f, 1f)] private float loseSoundVolume = 1f;

    [Header("Settings")]
<<<<<<< Updated upstream
    [Tooltip("UI kapandıktan sonra Lose ekranının gelmesi için geçecek süre.")]
    [SerializeField] private float loseDelay = 0.6f; // ChestHealthUI'da 0.5s beklemiştik, burada 0.6s idealdir.
=======
    [SerializeField] private float gameOverTextDelay = 3f;
    [SerializeField] private float returnToMainMenuDelay = 5f;

    private AudioSource audioSource;
    private bool loseTriggered;
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
        // Ekran geldikten SONRA oyunu tamamen durdur
        Time.timeScale = 0f;
=======
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
>>>>>>> Stashed changes
    }
}
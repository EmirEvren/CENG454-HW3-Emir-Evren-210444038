using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    private enum StagePhase
    {
        Loot,
        Combat,
        Transition,
        Completed
    }

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private StageLootSpawner lootSpawner;

    [Header("UI")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image timerImage;

    [Header("Timer UI Settings")]
    [SerializeField] private float timerTextAppearDelay = 0.2f;

    [Header("Stage Start Audio")]
    [SerializeField] private AudioClip stage1StartSound;
    [SerializeField] private AudioClip stage2StartSound;
    [SerializeField] private AudioClip stage3StartSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Range(0f, 1f)] private float stageStartSoundVolume = 1f;

    [Header("Stage Settings")]
    [SerializeField] private float lootDuration = 20f;
    [SerializeField] private float stageClearDelay = 2f;

    [SerializeField] private int[] enemiesPerStage = { 20, 30, 40 };
    [SerializeField] private int[] lootPickupCountPerColor = { 1, 1, 2 };
    [SerializeField] private int[] enemyDamagePerStage = { 10, 15, 15 };
    [SerializeField] private int[] enemyHealthPerStage = { 1, 1, 3 };

    public event Action<int> OnLootPhaseStarted;
    public event Action<int> OnCombatPhaseStarted;
    public event Action<int> OnStageCleared;
    public event Action OnAllStagesCompleted;

    private int currentStageIndex = 0;
    private float phaseTimer;
    private StagePhase currentPhase;

    private Coroutine timerShowRoutine;
    private Coroutine stageTransitionRoutine;

    private AudioSource audioSource;

    private int StageCount
    {
        get
        {
            return Mathf.Min(
                enemiesPerStage.Length,
                lootPickupCountPerColor.Length,
                enemyDamagePerStage.Length,
                enemyHealthPerStage.Length
            );
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = stageStartSoundVolume;

        if (sfxMixerGroup != null)
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    private void OnEnable()
    {
        if (enemySpawner != null)
            enemySpawner.OnWaveCompleted += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        if (enemySpawner != null)
            enemySpawner.OnWaveCompleted -= HandleWaveCompleted;

        if (stageTransitionRoutine != null)
        {
            StopCoroutine(stageTransitionRoutine);
            stageTransitionRoutine = null;
        }
    }

    private void Start()
    {
        currentStageIndex = 0;
        BeginLootPhase();
    }

    private void Update()
    {
        if (currentPhase != StagePhase.Loot)
            return;

        phaseTimer -= Time.deltaTime;

        if (phaseTimer < 0f)
            phaseTimer = 0f;

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(phaseTimer)}";

        if (phaseTimer <= 0f)
        {
            BeginCombatPhase();
        }
    }

    private void BeginLootPhase()
    {
        if (currentStageIndex >= StageCount)
        {
            CompleteAllStages();
            return;
        }

        currentPhase = StagePhase.Loot;
        phaseTimer = lootDuration;

        int stageNumber = currentStageIndex + 1;

        if (stageText != null)
            stageText.text = $"Stage {stageNumber}";

        if (phaseText != null)
            phaseText.text = "Loot Phase";

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(phaseTimer)}";

        ShowTimerUI();

        PlayStageStartSound(stageNumber);

        if (lootSpawner != null)
        {
            lootSpawner.ClearLoot();

            int pickupCount = lootPickupCountPerColor[currentStageIndex];
            lootSpawner.SpawnLootForStage(pickupCount);
        }

        OnLootPhaseStarted?.Invoke(stageNumber);

        Debug.Log($"Stage {stageNumber} Loot Phase started.");
    }

    private void BeginCombatPhase()
    {
        if (currentStageIndex >= StageCount)
        {
            CompleteAllStages();
            return;
        }

        currentPhase = StagePhase.Combat;

        int stageNumber = currentStageIndex + 1;

        if (stageText != null)
            stageText.text = $"Stage {stageNumber}";

        if (phaseText != null)
            phaseText.text = "Combat Phase";

        HideTimerUI();

        if (lootSpawner != null)
            lootSpawner.ClearLoot();

        if (enemySpawner != null)
        {
            int enemyCount = enemiesPerStage[currentStageIndex];
            int enemyDamage = enemyDamagePerStage[currentStageIndex];
            int enemyHealth = enemyHealthPerStage[currentStageIndex];

            enemySpawner.StartWave(enemyCount, enemyDamage, enemyHealth);
        }

        OnCombatPhaseStarted?.Invoke(stageNumber);

        Debug.Log($"Stage {stageNumber} Combat Phase started.");
    }

    private void HandleWaveCompleted()
    {
        if (currentPhase != StagePhase.Combat)
            return;

        int clearedStageNumber = currentStageIndex + 1;

        Debug.Log($"Stage {clearedStageNumber} Combat Phase completed.");

        currentPhase = StagePhase.Transition;

        if (phaseText != null)
            phaseText.text = "Stage Cleared";

        OnStageCleared?.Invoke(clearedStageNumber);

        if (stageTransitionRoutine != null)
            StopCoroutine(stageTransitionRoutine);

        stageTransitionRoutine = StartCoroutine(StageClearedRoutine());
    }

    private IEnumerator StageClearedRoutine()
    {
        yield return new WaitForSeconds(stageClearDelay);

        currentStageIndex++;

        if (currentStageIndex >= StageCount)
        {
            CompleteAllStages();
        }
        else
        {
            BeginLootPhase();
        }

        stageTransitionRoutine = null;
    }

    private void CompleteAllStages()
    {
        currentPhase = StagePhase.Completed;

        if (lootSpawner != null)
            lootSpawner.ClearLoot();

        if (stageText != null)
            stageText.text = "All Stages Clear";

        if (phaseText != null)
            phaseText.text = "You Win";

        HideTimerUI();

        OnAllStagesCompleted?.Invoke();

        Debug.Log("All stages completed. You Win.");
    }

    private void PlayStageStartSound(int stageNumber)
    {
        if (audioSource == null)
            return;

        AudioClip selectedClip = null;

        switch (stageNumber)
        {
            case 1:
                selectedClip = stage1StartSound;
                break;

            case 2:
                selectedClip = stage2StartSound;
                break;

            case 3:
                selectedClip = stage3StartSound;
                break;
        }

        if (selectedClip == null)
            return;

        audioSource.PlayOneShot(selectedClip, stageStartSoundVolume);
    }

    private void ShowTimerUI()
    {
        if (timerShowRoutine != null)
            StopCoroutine(timerShowRoutine);

        timerShowRoutine = StartCoroutine(ShowTimerUICoroutine());
    }

    private IEnumerator ShowTimerUICoroutine()
    {
        if (timerImage != null)
            timerImage.gameObject.SetActive(true);

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        yield return new WaitForSeconds(timerTextAppearDelay);

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        timerShowRoutine = null;
    }

    private void HideTimerUI()
    {
        if (timerShowRoutine != null)
        {
            StopCoroutine(timerShowRoutine);
            timerShowRoutine = null;
        }

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }

        if (timerImage != null)
            timerImage.gameObject.SetActive(false);
    }
}
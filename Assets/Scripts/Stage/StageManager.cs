using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageManager : MonoBehaviour
{
    private enum StagePhase
    {
        Loot,
        Combat,
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

    [Header("Stage Settings")]
    [SerializeField] private float lootDuration = 20f;

    [SerializeField] private int[] enemiesPerStage = { 20, 30, 40 };
    [SerializeField] private int[] lootPickupCountPerColor = { 1, 1, 2 };
    [SerializeField] private int[] enemyDamagePerStage = { 10, 15, 15 };
    [SerializeField] private int[] enemyHealthPerStage = { 1, 1, 3 };

    private int currentStageIndex = 0;
    private float phaseTimer;
    private StagePhase currentPhase;

    private Coroutine timerShowRoutine;

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

    private void OnEnable()
    {
        if (enemySpawner != null)
            enemySpawner.OnWaveCompleted += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        if (enemySpawner != null)
            enemySpawner.OnWaveCompleted -= HandleWaveCompleted;
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

        if (stageText != null)
            stageText.text = $"Stage {currentStageIndex + 1}";

        if (phaseText != null)
            phaseText.text = "Loot Phase";

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(phaseTimer)}";

        ShowTimerUI();

        if (lootSpawner != null)
        {
            lootSpawner.ClearLoot();

            int pickupCount = lootPickupCountPerColor[currentStageIndex];
            lootSpawner.SpawnLootForStage(pickupCount);
        }

        Debug.Log($"Stage {currentStageIndex + 1} Loot Phase started.");
    }

    private void BeginCombatPhase()
    {
        if (currentStageIndex >= StageCount)
        {
            CompleteAllStages();
            return;
        }

        currentPhase = StagePhase.Combat;

        if (stageText != null)
            stageText.text = $"Stage {currentStageIndex + 1}";

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

        Debug.Log($"Stage {currentStageIndex + 1} Combat Phase started.");
    }

    private void HandleWaveCompleted()
    {
        if (currentPhase != StagePhase.Combat)
            return;

        Debug.Log($"Stage {currentStageIndex + 1} Combat Phase completed.");

        currentStageIndex++;

        if (currentStageIndex >= StageCount)
        {
            CompleteAllStages();
            return;
        }

        BeginLootPhase();
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

        Debug.Log("All stages completed. You Win.");
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
using TMPro;
using UnityEngine;

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

    [Header("Stage Settings")]
    [SerializeField] private float lootDuration = 20f;

    [SerializeField] private int[] enemiesPerStage = { 20, 30, 40 };
    [SerializeField] private int[] lootPickupCountPerColor = { 1, 1, 2 };
    [SerializeField] private int[] enemyDamagePerStage = { 10, 15, 15 };
    [SerializeField] private int[] enemyHealthPerStage = { 1, 1, 3 };

    private int currentStageIndex;
    private float phaseTimer;
    private StagePhase currentPhase;

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

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(phaseTimer)}";

        if (phaseTimer <= 0f)
        {
            BeginCombatPhase();
        }
    }

    private void BeginLootPhase()
    {
        currentPhase = StagePhase.Loot;
        phaseTimer = lootDuration;

        if (stageText != null)
            stageText.text = $"Stage {currentStageIndex + 1}";

        if (phaseText != null)
            phaseText.text = "Loot Phase";

        if (lootSpawner != null)
        {
            int pickupCount = lootPickupCountPerColor[currentStageIndex];
            lootSpawner.SpawnLootForStage(pickupCount);
        }
    }

    private void BeginCombatPhase()
    {
        currentPhase = StagePhase.Combat;

        if (phaseText != null)
            phaseText.text = "Combat Phase";

        if (timerText != null)
            timerText.text = "";

        if (lootSpawner != null)
            lootSpawner.ClearLoot();

        if (enemySpawner != null)
        {
            int enemyCount = enemiesPerStage[currentStageIndex];
            int enemyDamage = enemyDamagePerStage[currentStageIndex];
            int enemyHealth = enemyHealthPerStage[currentStageIndex];

            enemySpawner.StartWave(enemyCount, enemyDamage, enemyHealth);
        }
    }

    private void HandleWaveCompleted()
    {
        currentStageIndex++;

        if (currentStageIndex >= enemiesPerStage.Length)
        {
            currentPhase = StagePhase.Completed;

            if (stageText != null)
                stageText.text = "All Stages Clear";

            if (phaseText != null)
                phaseText.text = "You Win";

            if (timerText != null)
                timerText.text = "";

            return;
        }

        BeginLootPhase();
    }
}
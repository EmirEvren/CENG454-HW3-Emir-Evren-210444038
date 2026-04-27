using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject fighterPrefab;
    [SerializeField] private GameObject archerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 0.5f;

    [Header("Spawn Chances")]
    [SerializeField, Range(0f, 1f)] private float fighterChance = 0.6f;

    [Header("Outline Settings")]
    [SerializeField] private float enemyOutlineWidth = 10f;

    private int enemiesToSpawn;
    private int spawnedEnemies;
    private int aliveEnemies;

    private int currentEnemyDamage;
    private int currentEnemyHealth;

    public event Action OnWaveCompleted;

    public void StartWave(int enemyCount, int enemyDamage, int enemyHealth)
    {
        StopAllCoroutines();

        enemiesToSpawn = enemyCount;
        spawnedEnemies = 0;
        aliveEnemies = 0;

        currentEnemyDamage = enemyDamage;
        currentEnemyHealth = enemyHealth;

        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (spawnedEnemies < enemiesToSpawn)
        {
            SpawnEnemy();
            spawnedEnemies++;

            yield return new WaitForSeconds(spawnInterval);
        }

        if (aliveEnemies == 0)
        {
            OnWaveCompleted?.Invoke();
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        GameObject selectedPrefab = GetRandomEnemyPrefab();

        if (selectedPrefab == null)
            return;

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject enemyObj = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);

        AmmoColor randomColor = (AmmoColor)UnityEngine.Random.Range(0, 4);

        EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.ConfigureHealth(currentEnemyHealth);
            enemyHealth.ConfigureColor(randomColor);

            enemyHealth.OnEnemyDied += HandleEnemyDied;
            aliveEnemies++;
        }

        ApplyOutline(enemyObj, randomColor);

        EnemyController enemyController = enemyObj.GetComponent<EnemyController>();

        if (enemyController != null)
        {
            enemyController.ConfigureDamage(currentEnemyDamage);
            enemyController.InitializeTargets();
        }
    }

    private void ApplyOutline(GameObject enemyObj, AmmoColor ammoColor)
    {
        if (enemyObj == null)
            return;

        Outline outline = enemyObj.GetComponent<Outline>();

        if (outline == null)
            outline = enemyObj.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = enemyOutlineWidth;
        outline.OutlineColor = GetOutlineColor(ammoColor);
    }

    private Color GetOutlineColor(AmmoColor ammoColor)
    {
        switch (ammoColor)
        {
            case AmmoColor.Red:
                return Color.red;

            case AmmoColor.Yellow:
                return Color.yellow;

            case AmmoColor.Green:
                return Color.green;

            case AmmoColor.Blue:
                return Color.blue;

            case AmmoColor.None:
            default:
                return Color.white;
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        float randomValue = UnityEngine.Random.value;

        if (randomValue <= fighterChance)
        {
            return fighterPrefab;
        }
        else
        {
            return archerPrefab;
        }
    }

    private void HandleEnemyDied(EnemyHealth enemy)
    {
        enemy.OnEnemyDied -= HandleEnemyDied;
        aliveEnemies--;

        if (spawnedEnemies >= enemiesToSpawn && aliveEnemies <= 0)
        {
            OnWaveCompleted?.Invoke();
        }
    }
}
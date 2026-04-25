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

        EnemyHealth enemyHealth = enemyObj.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.ConfigureHealth(currentEnemyHealth);

            AmmoColor randomColor = (AmmoColor)UnityEngine.Random.Range(0, 4);
            enemyHealth.ConfigureColor(randomColor);

            enemyHealth.OnEnemyDied += HandleEnemyDied;
            aliveEnemies++;
        }

        EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.ConfigureDamage(currentEnemyDamage);
            enemyController.InitializeTargets();
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
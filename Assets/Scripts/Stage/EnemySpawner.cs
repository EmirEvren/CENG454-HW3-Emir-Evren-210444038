using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 0.5f;

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
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

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
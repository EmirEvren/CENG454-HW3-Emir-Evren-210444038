using System.Collections.Generic;
using UnityEngine;

public class StageLootSpawner : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    [SerializeField] private GameObject redPickupPrefab;
    [SerializeField] private GameObject yellowPickupPrefab;
    [SerializeField] private GameObject greenPickupPrefab;
    [SerializeField] private GameObject bluePickupPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] redSpawnPoints;
    [SerializeField] private Transform[] yellowSpawnPoints;
    [SerializeField] private Transform[] greenSpawnPoints;
    [SerializeField] private Transform[] blueSpawnPoints;

    private readonly List<GameObject> activeLoot = new List<GameObject>();

    public void SpawnLootForStage(int pickupCountPerColor)
    {
        ClearLoot();

        SpawnColorSet(redPickupPrefab, redSpawnPoints, pickupCountPerColor, "Red");
        SpawnColorSet(yellowPickupPrefab, yellowSpawnPoints, pickupCountPerColor, "Yellow");
        SpawnColorSet(greenPickupPrefab, greenSpawnPoints, pickupCountPerColor, "Green");
        SpawnColorSet(bluePickupPrefab, blueSpawnPoints, pickupCountPerColor, "Blue");
    }

    public void ClearLoot()
    {
        for (int i = 0; i < activeLoot.Count; i++)
        {
            if (activeLoot[i] != null)
            {
                Destroy(activeLoot[i]);
            }
        }

        activeLoot.Clear();
    }

    private void SpawnColorSet(GameObject pickupPrefab, Transform[] spawnPoints, int count, string colorName)
    {
        if (pickupPrefab == null)
        {
            Debug.LogWarning($"{colorName} pickup prefab is not assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"{colorName} spawn points are empty.");
            return;
        }

        int spawnCount = Mathf.Min(count, spawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            if (spawnPoints[i] == null)
            {
                Debug.LogWarning($"{colorName} spawn point index {i} is null.");
                continue;
            }

            GameObject lootObj = Instantiate(
                pickupPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            activeLoot.Add(lootObj);
        }

        Debug.Log($"{colorName} loot spawned: {spawnCount}");
    }
}
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

        SpawnColorSet(redPickupPrefab, redSpawnPoints, pickupCountPerColor);
        SpawnColorSet(yellowPickupPrefab, yellowSpawnPoints, pickupCountPerColor);
        SpawnColorSet(greenPickupPrefab, greenSpawnPoints, pickupCountPerColor);
        SpawnColorSet(bluePickupPrefab, blueSpawnPoints, pickupCountPerColor);
    }

    public void ClearLoot()
    {
        for (int i = 0; i < activeLoot.Count; i++)
        {
            if (activeLoot[i] != null)
                Destroy(activeLoot[i]);
        }

        activeLoot.Clear();
    }

    private void SpawnColorSet(GameObject pickupPrefab, Transform[] spawnPoints, int count)
    {
        if (pickupPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        int spawnCount = Mathf.Min(count, spawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject lootObj = Instantiate(
                pickupPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            activeLoot.Add(lootObj);
        }
    }
}
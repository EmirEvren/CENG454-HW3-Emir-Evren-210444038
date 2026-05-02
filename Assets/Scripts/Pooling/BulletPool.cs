using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [Serializable]
    private class BulletPoolEntry
    {
        public AmmoColor color;
        public GameObject prefab;
        [Min(0)] public int preloadCount = 15;
    }

    [Header("Bullet Prefabs")]
    [SerializeField] private BulletPoolEntry[] bulletEntries;

    [Header("Pool Settings")]
    [SerializeField] private Transform poolParent;
    [SerializeField] private bool expandWhenEmpty = true;

    private readonly Dictionary<AmmoColor, Queue<Bullet>> pools = new Dictionary<AmmoColor, Queue<Bullet>>();
    private readonly Dictionary<AmmoColor, GameObject> prefabLookup = new Dictionary<AmmoColor, GameObject>();

    private void Awake()
    {
        if (poolParent == null)
            poolParent = transform;

        BuildPool();
    }

    private void BuildPool()
    {
        pools.Clear();
        prefabLookup.Clear();

        if (bulletEntries == null)
            return;

        for (int i = 0; i < bulletEntries.Length; i++)
        {
            BulletPoolEntry entry = bulletEntries[i];

            if (entry == null || entry.prefab == null)
                continue;

            if (!pools.ContainsKey(entry.color))
                pools.Add(entry.color, new Queue<Bullet>());

            if (!prefabLookup.ContainsKey(entry.color))
                prefabLookup.Add(entry.color, entry.prefab);

            for (int j = 0; j < entry.preloadCount; j++)
            {
                Bullet bullet = CreateBullet(entry.color);

                if (bullet != null)
                    pools[entry.color].Enqueue(bullet);
            }
        }
    }

    public Bullet GetBullet(AmmoColor color, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(color))
        {
            Debug.LogWarning($"BulletPool: No pool exists for color {color}.");
            return null;
        }

        Bullet bullet = null;

        if (pools[color].Count > 0)
        {
            bullet = pools[color].Dequeue();
        }
        else if (expandWhenEmpty)
        {
            bullet = CreateBullet(color);
        }

        if (bullet == null)
            return null;

        GameObject bulletRoot = bullet.RootObject;

        bulletRoot.transform.SetPositionAndRotation(position, rotation);
        bulletRoot.SetActive(true);

        bullet.AssignPool(this);
        bullet.OnSpawnedFromPool();

        return bullet;
    }

    public void ReturnBullet(Bullet bullet)
    {
        if (bullet == null)
            return;

        AmmoColor color = bullet.AmmoColor;

        bullet.OnReturnedToPool();

        GameObject bulletRoot = bullet.RootObject;
        bulletRoot.SetActive(false);

        if (poolParent != null)
            bulletRoot.transform.SetParent(poolParent);

        if (!pools.ContainsKey(color))
            pools.Add(color, new Queue<Bullet>());

        pools[color].Enqueue(bullet);
    }

    private Bullet CreateBullet(AmmoColor color)
    {
        if (!prefabLookup.TryGetValue(color, out GameObject prefab) || prefab == null)
        {
            Debug.LogWarning($"BulletPool: Missing prefab for color {color}.");
            return null;
        }

        GameObject bulletObj = Instantiate(prefab);
        bulletObj.SetActive(false);

        if (poolParent != null)
            bulletObj.transform.SetParent(poolParent);

        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet == null)
            bullet = bulletObj.GetComponentInChildren<Bullet>();

        if (bullet == null)
        {
            Debug.LogError($"BulletPool: Prefab {prefab.name} does not contain a Bullet component.");
            Destroy(bulletObj);
            return null;
        }

        bullet.AssignPool(this);
        bullet.OnReturnedToPool();

        return bullet;
    }
}
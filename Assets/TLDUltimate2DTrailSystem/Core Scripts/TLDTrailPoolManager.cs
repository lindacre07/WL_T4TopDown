using UnityEngine;
using System.Collections.Generic;

namespace TLD.Trail2D
{
    /// <summary>
    /// TLD Trail Pool Manager - Handles object pooling for trail systems
    /// Improves performance by reusing trail objects instead of destroying/creating them
    /// </summary>
    public class TLDTrailPoolManager : MonoBehaviour
    {
        private static TLDTrailPoolManager instance;
        public static TLDTrailPoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("TLD Trail Pool Manager");
                    instance = go.AddComponent<TLDTrailPoolManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("Pool Settings")]
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private bool allowGrowth = true;

        [Header("Statistics")]
        [SerializeField] private int activeTrails = 0;
        [SerializeField] private int pooledTrails = 0;
        [SerializeField] private int totalCreated = 0;

        private Dictionary<string, Queue<GameObject>> trailPools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<GameObject, string> activeTrailTypes = new Dictionary<GameObject, string>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Get a trail from the pool or create a new one
        /// </summary>
        public GameObject GetTrail(GameObject prefab)
        {
            string poolKey = prefab.name;

            if (!trailPools.ContainsKey(poolKey))
            {
                trailPools[poolKey] = new Queue<GameObject>();
            }

            GameObject trail;

            if (trailPools[poolKey].Count > 0)
            {
                trail = trailPools[poolKey].Dequeue();
                trail.SetActive(true);
                pooledTrails--;
            }
            else
            {
                trail = Instantiate(prefab);
                trail.name = prefab.name + "_Pooled";
                totalCreated++;
            }

            activeTrailTypes[trail] = poolKey;
            activeTrails++;

            return trail;
        }

        /// <summary>
        /// Return a trail to the pool
        /// </summary>
        public void ReturnTrail(GameObject trail)
        {
            if (trail == null) return;

            if (activeTrailTypes.ContainsKey(trail))
            {
                string poolKey = activeTrailTypes[trail];

                // Check if pool is at max capacity
                if (trailPools[poolKey].Count >= maxPoolSize && !allowGrowth)
                {
                    Destroy(trail);
                    activeTrailTypes.Remove(trail);
                    activeTrails--;
                    return;
                }

                // Clear the trail before returning to pool
                TLDTrailRenderer trailRenderer = trail.GetComponent<TLDTrailRenderer>();
                if (trailRenderer != null)
                {
                    trailRenderer.ClearTrail();
                }

                trail.SetActive(false);
                trail.transform.SetParent(transform);
                trailPools[poolKey].Enqueue(trail);

                activeTrailTypes.Remove(trail);
                activeTrails--;
                pooledTrails++;
            }
            else
            {
                // Trail not managed by pool, just destroy it
                Destroy(trail);
            }
        }

        /// <summary>
        /// Pre-warm the pool with a specific prefab
        /// </summary>
        public void WarmPool(GameObject prefab, int count)
        {
            string poolKey = prefab.name;

            if (!trailPools.ContainsKey(poolKey))
            {
                trailPools[poolKey] = new Queue<GameObject>();
            }

            for (int i = 0; i < count; i++)
            {
                GameObject trail = Instantiate(prefab, transform);
                trail.name = prefab.name + "_Pooled";
                trail.SetActive(false);
                trailPools[poolKey].Enqueue(trail);
                totalCreated++;
                pooledTrails++;
            }
        }

        /// <summary>
        /// Clear all pools
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in trailPools.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject trail = pool.Dequeue();
                    if (trail != null)
                    {
                        Destroy(trail);
                    }
                }
            }

            trailPools.Clear();
            activeTrailTypes.Clear();
            pooledTrails = 0;
            activeTrails = 0;
        }

        /// <summary>
        /// Clear a specific pool
        /// </summary>
        public void ClearPool(string poolKey)
        {
            if (trailPools.ContainsKey(poolKey))
            {
                while (trailPools[poolKey].Count > 0)
                {
                    GameObject trail = trailPools[poolKey].Dequeue();
                    if (trail != null)
                    {
                        Destroy(trail);
                        pooledTrails--;
                    }
                }
            }
        }

        /// <summary>
        /// Get pool statistics
        /// </summary>
        public void GetPoolStats(out int active, out int pooled, out int total)
        {
            active = activeTrails;
            pooled = pooledTrails;
            total = totalCreated;
        }

        private void OnDestroy()
        {
            ClearAllPools();
        }

#if UNITY_EDITOR
        [ContextMenu("Debug: Print Pool Stats")]
        private void DebugPrintStats()
        {
            Debug.Log($"=== TLD Trail Pool Stats ===");
            Debug.Log($"Active Trails: {activeTrails}");
            Debug.Log($"Pooled Trails: {pooledTrails}");
            Debug.Log($"Total Created: {totalCreated}");
            Debug.Log($"Pool Types: {trailPools.Count}");
        }
#endif
    }
}
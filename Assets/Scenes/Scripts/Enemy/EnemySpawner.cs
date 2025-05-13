using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private EnemyData[] availableEnemies;

    private int remainingEnemiesToSpawn;
    private float currentHealthMultiplier = 1f;
    private NativeArray<float> spawnWeights;
    private float totalWeight;

    private void Awake()
    {
        InitializeSpawnWeights();
    }

    private void InitializeSpawnWeights()
    {
        // Calcule les poids de spawn basés sur la rareté
        spawnWeights = new NativeArray<float>(availableEnemies.Length, Allocator.Persistent);
        totalWeight = 0f;

        for (int i = 0; i < availableEnemies.Length; i++)
        {
            float weight = 1f / (6f - availableEnemies[i].spawnRarity); // Convertit la rareté en poids (1-5 devient ~0.2-1.0)
            spawnWeights[i] = weight;
            totalWeight += weight;
        }
    }

    public void StartSpawning(int count, float healthMultiplier)
    {
        remainingEnemiesToSpawn = count;
        currentHealthMultiplier = healthMultiplier;
        SpawnEnemyBatch();
    }

    [BurstCompile]
    private struct SpawnJob : IJob
    {
        public NativeArray<float> weights;
        public float totalWeight;
        public int enemyCount;
        [WriteOnly] public NativeArray<int> selectedIndices;
        public uint randomSeed;

        public void Execute()
        {
            // Use Unity.Mathematics Random for thread safety
            var random = new Unity.Mathematics.Random(randomSeed);
            
            for (int i = 0; i < enemyCount; i++)
            {
                float randomValue = random.NextFloat(0f, totalWeight);
                float currentSum = 0f;

                for (int j = 0; j < weights.Length; j++)
                {
                    currentSum += weights[j];
                    if (randomValue <= currentSum)
                    {
                        selectedIndices[i] = j;
                        break;
                    }
                }
            }
        }
    }

    private void SpawnEnemyBatch()
    {
        if (availableEnemies == null || availableEnemies.Length == 0)
        {
            Debug.LogError($"[EnemySpawner] No enemy prefabs assigned on {gameObject.name}");
            return;
        }
        
        int batchSize = Mathf.Min(remainingEnemiesToSpawn, 10); // Spawn par lots de 10 max
        
        // Prépare le job de sélection d'ennemis
        NativeArray<int> selectedIndices = new NativeArray<int>(batchSize, Allocator.TempJob);
        
        var spawnJob = new SpawnJob
        {
            weights = spawnWeights,
            totalWeight = totalWeight,
            enemyCount = batchSize,
            selectedIndices = selectedIndices,
            randomSeed = (uint)UnityEngine.Random.Range(1, 100000) // Generate seed on main thread
        };

        // Exécute le job
        JobHandle jobHandle = spawnJob.Schedule();
        jobHandle.Complete();

        // Spawn les ennemis sélectionnés
        for (int i = 0; i < batchSize; i++)
        {
            SpawnEnemy(selectedIndices[i]);
        }

        // Nettoie
        selectedIndices.Dispose();

        // Met à jour le compteur
        remainingEnemiesToSpawn -= batchSize;

        // Continue si nécessaire
        if (remainingEnemiesToSpawn > 0)
        {
            Invoke(nameof(SpawnEnemyBatch), 0.1f); // Petit délai entre les lots
        }
    }

    private void SpawnEnemy(int enemyIndex)
    {
        if (enemyIndex >= 0 && enemyIndex < availableEnemies.Length)
        {
            // Position aléatoire dans le rayon de spawn
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            // Instantie l'ennemi
            GameObject enemy = Instantiate(availableEnemies[enemyIndex].enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Applique le multiplicateur de santé
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                float baseHealth = availableEnemies[enemyIndex].maxHealth;
                enemyHealth.SetMaxHealth(baseHealth * currentHealthMultiplier);
            }
        }
    }

    private void OnDestroy()
    {
        if (spawnWeights.IsCreated)
        {
            spawnWeights.Dispose();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

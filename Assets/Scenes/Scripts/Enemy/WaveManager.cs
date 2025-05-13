using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float healthMultiplier = 1.1f;  // +10% par vague
    [SerializeField] private float timeBetweenWaves = 5f;
    
    [Header("References")]
    [SerializeField] private EnemySpawner[] spawners;

    private int currentWave = 0;
    private int enemiesPerWave;
    private bool isWaveActive = false;

    private void Start()
    {
        enemiesPerWave = baseEnemiesPerWave;
        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            if (!isWaveActive)
            {
                StartNewWave();
            }
            yield return new WaitForSeconds(1f); // Vérifie toutes les secondes
        }
    }

    private void StartNewWave()
    {
        currentWave++;
        isWaveActive = true;
        
        // Calcule le nombre d'ennemis pour cette vague
        enemiesPerWave = baseEnemiesPerWave + (currentWave - 1) * 2;

        // Distribue les ennemis entre les spawners
        int enemiesPerSpawner = enemiesPerWave / spawners.Length;
        int remainingEnemies = enemiesPerWave % spawners.Length;

        for (int i = 0; i < spawners.Length; i++)
        {
            int enemiesToSpawn = enemiesPerSpawner;
            if (remainingEnemies > 0)
            {
                enemiesToSpawn++;
                remainingEnemies--;
            }

            if (spawners[i] != null)
            {
                spawners[i].StartSpawning(enemiesToSpawn, GetCurrentHealthMultiplier());
            }
        }

        Debug.Log($"Wave {currentWave} started with {enemiesPerWave} enemies!");
    }

    public void EnemyDefeated()
    {
        enemiesPerWave--;
        if (enemiesPerWave <= 0)
        {
            WaveCompleted();
        }
    }

    private void WaveCompleted()
    {
        isWaveActive = false;
        Debug.Log($"Wave {currentWave} completed!");
        StartCoroutine(WaitForNextWave());
    }

    private IEnumerator WaitForNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNewWave();
    }

    private float GetCurrentHealthMultiplier()
    {
        return Mathf.Pow(healthMultiplier, currentWave - 1);
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}

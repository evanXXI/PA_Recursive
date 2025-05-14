using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public GameObject enemyPrefab;
    
    [Header("Spawn Settings")]
    [Range(1, 5)]
    [Tooltip("1 = très rare, 5 = très fréquent ; Kamikaze: 1, Normie: 5, Archer/Firespitter: 3, Tank: 2, Boss: unique - spawns after x amount of waves")]
    public int spawnRarity = 1;
    
    [Header("Stats")]
    [Tooltip("Kamikaze: 20, Normie: 60, Archer/Firespitter: 40, Tank: 200, Boss: 1000")]
    public float maxHealth = 100f;

    [Tooltip("Kamikaze: 8, Normie: 5, Archer/Firespitter: 5, Tank: 3, Boss: Variable")]
    public float moveSpeed = 5f;

    [Tooltip("Kamikaze/Tank: 1.0f (contact), Normie/Boss(proche): 1.5f, Archer/Firespitter/Boss(distance): 5.0f")]
    public float attackRange = 2f;

    [Tooltip("Kamikaze: 20, Normie: 10, Archer: 12, Firespitter: 15, Tank: 17")]
    public float baseDamage = 10f;

    [Header("Debug")]
    public bool showAttackRange = true;
    
    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
        attackRange = Mathf.Max(0.1f, attackRange);
        spawnRarity = Mathf.Clamp(spawnRarity, 1, 5);
    }
}

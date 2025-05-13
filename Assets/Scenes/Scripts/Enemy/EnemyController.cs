using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;
    
    [Header("UI Settings")]
    [SerializeField] private GameObject healthBarPrefab;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float cooldownRandomness = 0.3f;  // Variation +/- du cooldown
    
    private Transform player;
    private Health health;
    private float distanceToPlayer;
    private bool canAttack = true;
    private string enemyTag;

    private void Start()
    {
        // Récupère les composants et références
        health = GetComponent<Health>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyTag = gameObject.tag;

        // Applique les stats depuis EnemyData
        if (enemyData != null)
        {
            health.SetMaxHealth(enemyData.maxHealth);
        }

        // Initialise la barre de vie
        if (healthBarPrefab != null)
        {
            GameObject healthBarInstance = Instantiate(healthBarPrefab, transform);
            HealthBarUI healthBar = healthBarInstance.GetComponentInChildren<HealthBarUI>();
            if (healthBar != null)
            {
                healthBar.Initialize(health);
            }
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Calcule la distance avec le joueur
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Vérifie si l'ennemi est à portée d'attaque
        if (distanceToPlayer <= enemyData.attackRange && canAttack)
        {
            Attack();
            StartCoroutine(AttackCooldown());
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        // Les Archers et Firespitters restent à distance
        if (enemyTag == "ArcherTag" || enemyTag == "FirespitterTag")
        {
            if (distanceToPlayer < enemyData.attackRange * 0.8f)
            {
                // S'éloigne du joueur
                Vector3 direction = (transform.position - player.position).normalized;
                transform.position += direction * enemyData.moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            // Calcule la direction vers le joueur
            Vector3 direction = (player.position - transform.position).normalized;
            
            // Les Tanks sont plus lents mais ont plus de vie
            float speedMultiplier = enemyTag == "TankTag" ? 0.5f : 1f;
            
            // Déplace l'ennemi
            transform.position += direction * enemyData.moveSpeed * speedMultiplier * Time.deltaTime;
        }
        
        // Fait regarder l'ennemi vers le joueur
        transform.LookAt(player);
    }

    private void Attack()
    {
        if (!canAttack) return;

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth == null) return;

        float damage = enemyData.baseDamage;
        
        switch (enemyTag)
        {
            case "KamikazeTag":
                // Le Kamikaze fait des dégâts explosifs et meurt
                damage *= 2f;
                playerHealth.TakeDamage(damage);
                health.TakeDamage(health.GetMaxHealth()); // Suicide
                break;

            case "TankTag":
                // Le Tank fait moins de dégâts mais a plus de vie
                damage *= 0.7f;
                playerHealth.TakeDamage(damage);
                break;

            case "ArcherTag":
            case "FirespitterTag":
                // Les attaquants à distance font des dégâts moyens
                damage *= 0.8f;
                playerHealth.TakeDamage(damage);
                break;

            case "NormieTag":
            default:
                // Dégâts normaux
                playerHealth.TakeDamage(damage);
                break;
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        
        // Ajuste le cooldown selon le type d'ennemi
        float adjustedCooldown = attackCooldown;
        switch (enemyTag)
        {
            case "KamikazeTag":
                adjustedCooldown *= 0.5f; // Plus rapide
                break;
            case "TankTag":
                adjustedCooldown *= 1.5f; // Plus lent
                break;
            case "ArcherTag":
            case "FirespitterTag":
                adjustedCooldown *= 1.2f; // Légèrement plus lent
                break;
        }
        
        // Cooldown avec variation aléatoire
        float randomCooldown = adjustedCooldown + Random.Range(-cooldownRandomness, cooldownRandomness);
        yield return new WaitForSeconds(Mathf.Max(0.1f, randomCooldown));
        canAttack = true;
    }

    // Pour le debug
    private void OnDrawGizmosSelected()
    {
        if (enemyData != null && enemyData.showAttackRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);
        }
    }
}

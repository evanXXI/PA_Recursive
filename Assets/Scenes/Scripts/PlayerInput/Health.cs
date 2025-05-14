using UnityEngine;
using System;

public class EnhancedHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool isDead = false;

    private float currentHealth;

    // Événement pour notifier les changements de santé
    public event Action<float, float> OnHealthChanged;
    
    // Événement pour notifier la mort
    public event Action OnDeath;

    // Propriétés pour accéder aux valeurs
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // Notifier les abonnés de l'état initial
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Définit la santé maximale et réinitialise la santé actuelle à cette valeur
    /// </summary>
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);
        currentHealth = maxHealth;
        isDead = false;
        
        // Notifier les abonnés
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Inflige des dégâts à l'entité
    /// </summary>
    public void TakeDamage(float damageAmount, GameObject instigator = null)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
        
        // Notifier les abonnés
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Vérifier si l'entité est morte
        if (currentHealth <= 0f && !isDead)
        {
            Die(instigator);
        }
    }

    /// <summary>
    /// Gère la mort de l'entité
    /// </summary>
    private void Die(GameObject instigator = null)
    {
        isDead = true;
        
        // Notifier les abonnés
        OnDeath?.Invoke();

        // Chercher un gestionnaire de mort spécifique
        IDeath deathHandler = GetComponent<IDeath>();
        if (deathHandler != null)
        {
            deathHandler.HandleDeath(instigator);
        }
        else
        {
            // Comportement de mort par défaut
            Destroy(gameObject, 0.1f);
        }
    }
}

// Interface pour personnaliser la gestion de la mort
public interface IDeath
{
    void HandleDeath(GameObject instigator = null);
} 
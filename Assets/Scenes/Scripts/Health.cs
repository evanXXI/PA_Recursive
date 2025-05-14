using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent onDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (onDeath == null)
            onDeath = new UnityEvent();
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = Mathf.Max(0f, amount);
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        
        if (currentHealth <= 0f)
        {
            onDeath.Invoke();
            Destroy(this.gameObject);
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(0f, maxHealth);
        currentHealth = Mathf.Max(0f, currentHealth);
    }
}



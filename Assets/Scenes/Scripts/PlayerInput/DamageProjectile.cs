using UnityEngine;

public class DamageProjectile : bullet
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float explosionRadius = 0f;  // 0 pour pas d'explosion
    
    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }
    
    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
    }
    
    public override void impact(Collision other)
    {
        // Instancier les particules comme dans la classe parente
        Instantiate(particles, other.contacts[0].point, transform.rotation);
        
        // Si c'est une explosion, appliquer des dégâts dans un rayon
        if (explosionRadius > 0)
        {
            // Trouver tous les objets dans le rayon d'explosion
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            
            foreach (Collider hit in colliders)
            {
                ApplyDamage(hit.gameObject);
            }
        }
        else
        {
            // Sinon, appliquer des dégâts directs à l'objet touché
            ApplyDamage(other.gameObject);
        }
        
        // Détruire le projectile
        Destroy(gameObject);
    }
    
    private void ApplyDamage(GameObject target)
    {
        if (target == owner) return;
        
        // Essayer d'abord avec le système EnhancedHealth
        if (target.TryGetComponent(out EnhancedHealth healthComponent))
        {
            healthComponent.TakeDamage(damageAmount);
            return;
        }
        
        // Essayer avec le système Health standard
        if (target.TryGetComponent(out Health healthComponent2))
        {
            healthComponent2.TakeDamage(damageAmount);
            return;
        }
        
        // Si pas de Health, essayer avec damageable (pour compatibilité)
        if (target.TryGetComponent(out damageable damageableObject))
        {
            damageableObject.damage(damageAmount);
        }
    }
} 
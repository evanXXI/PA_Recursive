using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class TopDownAttackAdapter : MonoBehaviour
{
    [Header("Attack References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject ammo1Prefab; // bullet.prefab
    [SerializeField] private GameObject ammo2Prefab; // explosion.prefab
    [SerializeField] private GameObject ammo3Prefab; // bouncyBullet.prefab
    
    [Header("Attack Settings")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwUpwardForce = 0f;
    
    [Header("Damage Settings")]
    [SerializeField] private float standardDamage = 10f;
    [SerializeField] private float explosiveDamage = 15f;
    [SerializeField] private float bouncyDamage = 7f;
    
    [Header("Aim Settings")]
    [SerializeField] private bool useMouseAiming = true;
    [SerializeField] private float aimDistance = 10f;
    [SerializeField] private bool useDirectMouseInput = true;
    [SerializeField] private bool showDebug = true; // Afficher les visuels de debug
    
    // Variables privées
    private GameObject currentAmmoPrefab;
    private int currentAmmoType = 1;
    private float currentDamage;
    private Vector3 aimDirection;
    private Vector2 lookInput;
    private Camera mainCamera;
    private Vector3 lastHitPoint; // Dernier point d'impact pour le debug
    
    private void Awake()
    {
        // Initialisation
        currentAmmoPrefab = ammo1Prefab;
        currentDamage = standardDamage;
        mainCamera = Camera.main;
        aimDirection = transform.forward;
        lastHitPoint = transform.position + transform.forward * aimDistance;
        
        // Créer un point d'attaque s'il n'existe pas
        if (attackPoint == null)
        {
            attackPoint = new GameObject("AttackPoint").transform;
            attackPoint.SetParent(transform);
            attackPoint.localPosition = new Vector3(0, 0.5f, 0.5f);
        }
    }
    
    private void Update()
    {
        // Si on utilise l'entrée directe de la souris, mettre à jour lookInput
        if (useDirectMouseInput && useMouseAiming)
        {
            lookInput = Input.mousePosition;
        }
        
        UpdateAimDirection();
    }
    
    // Méthode pour mettre à jour la position de visée à partir des inputs
    public void UpdateAimPosition(Vector2 inputLook)
    {
        // N'utiliser les inputs externes que si on n'utilise pas l'entrée directe de la souris
        if (!useDirectMouseInput || !useMouseAiming)
        {
            lookInput = inputLook;
        }
    }
    
    // Méthode pour calculer la direction de visée
    private void UpdateAimDirection()
    {
        if (useMouseAiming && mainCamera != null)
        {
            // Convertir la position de la souris en point dans le monde
            Ray ray = mainCamera.ScreenPointToRay(lookInput);
            
            // Créer un plan à la hauteur du joueur
            Plane groundPlane = new Plane(Vector3.up, transform.position.y);
            
            // Si le rayon touche le plan, on obtient le point d'intersection
            if (groundPlane.Raycast(ray, out float hitDistance))
            {
                // Point d'intersection du rayon avec le plan
                Vector3 hitPoint = ray.GetPoint(hitDistance);
                lastHitPoint = hitPoint; // Sauvegarder pour le debug
                
                // Direction du joueur vers le point (en ignorant la hauteur)
                Vector3 direction = hitPoint - transform.position;
                direction.y = 0;
                
                // Si la direction est valide (pas trop proche du joueur)
                if (direction.sqrMagnitude > 0.001f)
                {
                    // Normaliser la direction
                    aimDirection = direction.normalized;
                    
                    // Faire tourner le joueur vers cette direction
                    transform.rotation = Quaternion.LookRotation(aimDirection);
                }
                
                // Afficher les rayons de debug
                if (showDebug)
                {
                    Debug.DrawRay(ray.origin, ray.direction * hitDistance, Color.yellow);
                    Debug.DrawLine(transform.position, hitPoint, Color.red);
                    Debug.DrawRay(transform.position, aimDirection * aimDistance, Color.blue);
                }
            }
        }
        else
        {
            // Visée avec le joystick
            if (lookInput.magnitude > 0.1f)
            {
                // Convertir l'input du joystick en direction 3D
                aimDirection = new Vector3(lookInput.x, 0, lookInput.y).normalized;
                
                // Calculer le point de visée
                Vector3 aimPoint = transform.position + aimDirection * aimDistance;
                lastHitPoint = aimPoint;
                
                // Faire tourner le joueur vers cette direction
                transform.rotation = Quaternion.LookRotation(aimDirection);
                
                // Afficher les rayons de debug
                if (showDebug)
                {
                    Debug.DrawLine(transform.position, aimPoint, Color.blue);
                }
            }
        }
    }
    
    // Méthode appelée par l'InputManager pour tirer
    public void Shoot()
    {
        if (currentAmmoPrefab == null) return;
        
        // Instantier le projectile et obtenir ses composants
        GameObject projectile = Instantiate(currentAmmoPrefab, attackPoint.position, Quaternion.LookRotation(aimDirection));
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb == null) return;
        
        // Ajouter la force dans la direction de visée
        projectileRb.AddForce(aimDirection * throwForce + Vector3.up * throwUpwardForce, ForceMode.Impulse);
        
        // Définir le propriétaire et les dégâts
        if (projectile.TryGetComponent(out projectile projectileScript))
        {
            projectileScript.owner = gameObject;
        }
        
        if (projectile.TryGetComponent(out DamageProjectile damageProjectile))
        {
            damageProjectile.SetDamage(currentDamage);
        }
    }
    
    // Méthode appelée par l'InputManager pour changer de type de munition
    public void SwitchAmmo()
    {
        currentAmmoType = (currentAmmoType % 3) + 1;
        
        switch (currentAmmoType)
        {
            case 1: // Standard
                currentAmmoPrefab = ammo1Prefab;
                throwForce = 15f;
                currentDamage = standardDamage;
                break;
            case 2: // Explosive
                currentAmmoPrefab = ammo2Prefab;
                throwForce = 7f;
                currentDamage = explosiveDamage;
                break;
            case 3: // Bouncy
                currentAmmoPrefab = ammo3Prefab;
                throwForce = 10f;
                currentDamage = bouncyDamage;
                break;
        }
    }
    
    // Dessiner des gizmos pour visualiser la direction de visée
    private void OnDrawGizmos()
    {
        if (!showDebug) return;
        
        // Dessiner la direction de visée
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + aimDirection * aimDistance);
        
        // Dessiner le point d'impact du rayon
        if (useMouseAiming)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastHitPoint, 0.3f);
        }
    }
} 
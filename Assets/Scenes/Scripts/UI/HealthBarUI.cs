using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health healthComponent;
    [SerializeField] private Image fillImage;
    
    [Header("Settings")]
    [SerializeField] private bool isWorldSpace = true;  // False pour UI player
    [SerializeField] private bool useBillboard = true;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);
    
    private Camera mainCamera;
    private Transform parentTransform;
    private Canvas canvas;
    private bool isInitialized = false;

    public void Initialize(Health health)
    {
        if (health == null)
        {
            Debug.LogError($"[HealthBarUI] Initialize called with null Health component on {gameObject.name}");
            return;
        }

        // Try to find the fillImage if it's not assigned
        if (fillImage == null)
        {
            // Try to find it in children
            fillImage = GetComponentInChildren<Image>();
            
            // If still null, look for a specific name pattern
            if (fillImage == null)
            {
                Transform fillTransform = transform.Find("HealthBar_Fill");
                if (fillTransform != null)
                {
                    fillImage = fillTransform.GetComponent<Image>();
                }
            }
            
            // If we still can't find it, log error but continue
            if (fillImage == null)
            {
                Debug.LogError($"[HealthBarUI] Fill Image is not assigned in the inspector on {gameObject.name}");
                // We'll continue anyway in case fillImage gets assigned later
            }
        }

        healthComponent = health;
        healthComponent.onDeath.AddListener(HandleDeath);
        isInitialized = true;
        
        if (isWorldSpace)
        {
            parentTransform = healthComponent.transform;
            UpdatePosition();
        }
    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        
        // Pour la barre de vie du player
        if (!isWorldSpace && !isInitialized)
        {
            // Cherche le composant Health du player dans la scène
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError($"[HealthBarUI] No GameObject with tag 'Player' found in the scene. Make sure your player has the 'Player' tag. GameObject: {gameObject.name}");
                enabled = false;
                return;
            }

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth == null)
            {
                Debug.LogError($"[HealthBarUI] The Player GameObject exists but has no Health component attached. GameObject requesting health: {gameObject.name}");
                Debug.LogError($"[HealthBarUI] Please add a Health component to the Player GameObject named: {player.name}");
                enabled = false;
                return;
            }

            Initialize(playerHealth);
        }
        // Pour les barres de vie des ennemis
        else if (isWorldSpace && !isInitialized)
        {
            Transform parent = transform.parent;
            if (parent == null)
            {
                Debug.LogError($"[HealthBarUI] No parent transform found for world space health bar on {gameObject.name}");
                enabled = false;
                return;
            }

            // Cherche le composant Health dans toute la hiérarchie parente
            Health parentHealth = null;
            Transform currentTransform = parent;
            while (currentTransform != null && parentHealth == null)
            {
                parentHealth = currentTransform.GetComponent<Health>();
                if (parentHealth == null)
                {
                    currentTransform = currentTransform.parent;
                }
            }

            if (parentHealth == null)
            {
                Debug.LogError($"[HealthBarUI] No Health component found in parent hierarchy of {gameObject.name}");
                enabled = false;
                return;
            }

            Initialize(parentHealth);
        }

        if (!isInitialized)
        {
            Debug.LogWarning($"[HealthBarUI] Not initialized on {gameObject.name}. Make sure to call Initialize() or set Health component in the inspector.");
            enabled = false;
            return;
        }
    }

    private void LateUpdate()
    {
        if (healthComponent == null) return;

        // Met à jour le remplissage
        if (fillImage != null)
        {
            fillImage.fillAmount = healthComponent.GetCurrentHealth() / healthComponent.GetMaxHealth();
        }

        // Gestion World Space uniquement
        if (isWorldSpace)
        {
            if (useBillboard && mainCamera != null)
            {
                transform.rotation = mainCamera.transform.rotation;
            }
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        if (parentTransform != null)
        {
            transform.position = parentTransform.position + offset;
        }
    }

    private void HandleDeath()
    {
        if (isWorldSpace)
        {
            gameObject.SetActive(false);
        }
        else if (fillImage != null)
        {
            fillImage.fillAmount = 0;
        }
    }

    private void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.onDeath.RemoveListener(HandleDeath);
        }
    }
}

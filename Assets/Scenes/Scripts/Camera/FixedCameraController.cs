using UnityEngine;

public class FixedCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.1f; // Temps d'interpolation (plus petit = plus réactif)
    [SerializeField] private bool showDebug = true; // Afficher les visuels de debug
    
    // Variables privées
    private Vector3 velocity = Vector3.zero;
    private Vector3 cameraOffset;
    
    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        // Calculer l'offset initial basé sur la position actuelle de la caméra
        if (target != null)
        {
            cameraOffset = transform.position - target.position;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculer la position cible en maintenant l'offset initial
        Vector3 targetPosition = target.position + cameraOffset;
        
        // Afficher les visuels de debug
        if (showDebug)
        {
            Debug.DrawLine(transform.position, targetPosition, Color.green);
            Debug.DrawLine(target.position, target.position + Vector3.up * 2f, Color.blue);
        }
        
        // Utiliser SmoothDamp pour un mouvement fluide
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            smoothTime);
    }
    
    private void OnDrawGizmos()
    {
        if (target != null && showDebug)
        {
            // Dessiner une sphère à la position du joueur
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position, 0.5f);
            
            // Dessiner une ligne représentant l'offset
            Gizmos.color = Color.green;
            
            // Utiliser soit l'offset calculé s'il est disponible, soit calculer un offset temporaire
            Vector3 offset = Application.isPlaying ? cameraOffset : (transform.position - target.position);
            Gizmos.DrawLine(target.position, target.position + offset);
        }
    }
} 
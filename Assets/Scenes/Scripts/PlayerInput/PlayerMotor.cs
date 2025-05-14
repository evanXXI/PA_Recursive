using System;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    
    public float speed = 5f;
    public float gravity = -9.8f;
    
    private Camera mainCamera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }
    
    //Receive movement from the InputManager.cs and apply them to our character controller
    public void ProcessMove(Vector2 input)
    {
        // Créer un vecteur de direction basé sur les entrées
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        
        if (mainCamera != null)
        {
            // Convertir le mouvement pour qu'il soit relatif à la caméra
            // Ignorer la rotation sur X et Z, garder seulement Y (rotation horizontale)
            Quaternion camRotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);
            moveDirection = camRotation * moveDirection;
        }
        
        // Appliquer le mouvement
        controller.Move(moveDirection * speed * Time.deltaTime);
        
        // Appliquer la gravité
        playerVelocity.y += gravity * Time.deltaTime;
        playerVelocity.y = Mathf.Max(playerVelocity.y, -9.8f); // Limiter la vitesse de chute
        controller.Move(playerVelocity * Time.deltaTime);
    }
}

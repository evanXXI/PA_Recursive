using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    
    private PlayerMotor motor;
    private TopDownAttackAdapter attackAdapter;
    
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        
        motor = GetComponent<PlayerMotor>();
        attackAdapter = GetComponent<TopDownAttackAdapter>();
        
        // Callbacks pour les actions d'attaque
        if (attackAdapter != null)
        {
            onFoot.Shoot.performed += ctx => attackAdapter.Shoot();
            onFoot.SwitchAmmo.performed += ctx => attackAdapter.SwitchAmmo();
        }
    }

    void Update()
    {
        // Transmettre la position de visée à l'adaptateur d'attaque
        if (attackAdapter != null)
        {
            attackAdapter.UpdateAimPosition(onFoot.Look.ReadValue<Vector2>());
        }
    }

    void FixedUpdate()
    {
        // Tell the PlayerMotor to move using the value from our movement action.
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
     }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isAttacking;
    private float speed;
    private float atkSpeed;

    private void Start()
    {
        speed = DataPlayer.Instace.moveSpeed;
        atkSpeed = DataPlayer.Instace.ataqueSpeed;
    }

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Ataque.performed += ctx => Attack();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Debug.Log("Ataque iniciado!");
            // Aqui você pode disparar animação, som ou lógica de ataque

            // Simulando fim do ataque depois de 0.5s
            Invoke(nameof(EndAttack), atkSpeed);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Ataque finalizado!");
    }
}
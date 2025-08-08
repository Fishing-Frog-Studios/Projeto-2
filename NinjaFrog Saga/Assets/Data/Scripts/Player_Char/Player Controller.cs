using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isAttacking;
    private bool isMenuOpen = false;

    [Header("Dados do Player (ScriptableObject)")]
    public DataPlayerSO dataPlayer;

    private float speed;
    private float atkSpeed;

    [Header("Referência da UI do Menu")]
    public GameObject menuUI;

    private Interactable currentInteractable;

    private void Awake()
    {
        controls = new PlayerControls();

        // Movimento
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Ataque
        controls.Player.Ataque.performed += ctx => Attack();

        // Menu
        controls.Player.Menu.performed += ctx => ToggleMenu();

        // Interação
        controls.Player.Interact.performed += ctx => Interact();
    }

    private void Start()
    {
        if (dataPlayer != null)
        {
            speed = dataPlayer.moveSpeed;
            atkSpeed = dataPlayer.ataqueSpeed;
        }
        else
        {
            Debug.LogWarning("DataPlayerSO não está atribuído no PlayerController.");
            speed = 5f;
            atkSpeed = 0.5f;
        }

        if (menuUI != null)
            menuUI.SetActive(false);

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
        if (isMenuOpen) return;

        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void Attack()
    {
        if (isMenuOpen || isAttacking) return;

        isAttacking = true;
        Debug.Log("Ataque iniciado!");
        Invoke(nameof(EndAttack), atkSpeed);
    }

    private void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Ataque finalizado!");
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuUI != null)
            menuUI.SetActive(isMenuOpen);

        Time.timeScale = isMenuOpen ? 0f : 1f;

    }

    // ---------------------------
    //     SISTEMA DE INTERAÇÃO
    // ---------------------------

    private void Interact()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteractInput();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interact))
        {
            currentInteractable = interact;
            Debug.Log("Player pode interagir com " + interact.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interact))
        {
            if (currentInteractable == interact)
            {
                currentInteractable = null;
                Debug.Log("Player saiu do alcance de interação.");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isAttacking;
    private float speed;
    private float atkSpeed;

    private bool isMenuOpen = false;

    [Header("Refer�ncia da UI do Menu")]
    public GameObject menuUI; // arraste seu painel de menu aqui no Inspector

    private void Awake()
    {
        // Instancia os controles do novo Input System
        controls = new PlayerControls();

        // Captura o movimento do jogador
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Captura o ataque
        controls.Player.Ataque.performed += ctx => Attack();

        // Captura o bot�o de menu
        controls.Player.Menu.performed += ctx => ToggleMenu();
    }

    private void Start()
    {
        // Pega os dados do player de um script externo
        speed = DataPlayer.Instace.moveSpeed;
        atkSpeed = DataPlayer.Instace.ataqueSpeed;

        // Garante que o menu come�a fechado
        menuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //
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
        // Se o menu estiver aberto, n�o move
        if (isMenuOpen) return;


        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void Attack()
    {
        // Se o menu estiver aberto ou j� estiver atacando, n�o faz nada
        if (isMenuOpen || isAttacking) return;

        isAttacking = true;
        Debug.Log("Ataque iniciado!");

        // Aqui voc� pode disparar anima��o, som, etc.
        Invoke(nameof(EndAttack), atkSpeed);
    }

    private void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Ataque finalizado!");
    }

    private void ToggleMenu()
    {
        // Alterna o estado do menu
        isMenuOpen = !isMenuOpen;

        // Ativa ou desativa o painel da UI
        menuUI.SetActive(isMenuOpen);

        // Pausa ou despausa o jogo
        Time.timeScale = isMenuOpen ? 0f : 1f;

        // Controla a visibilidade e bloqueio do cursor
        Cursor.visible = isMenuOpen;
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}

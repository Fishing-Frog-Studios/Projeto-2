using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
public class PlayerControllerMOdificado : MonoBehaviour
{
    public static PlayerControllerMOdificado Instance { get; private set; }
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isAttacking;
    private float speed;
    private float atkSpeed;

    private bool isMenuOpen = false;

    [Header("Referencia da UI do Menu")]
    public GameObject menuUI; // arraste seu painel de menu aqui no Inspector

    //madao new
    [Header("UI de Debug")]
    public TMP_Text debugText;
    
    

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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

    // Garante que o menu começa fechado
    menuUI.SetActive(false);
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
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

        //madao new
        //essa é a parte das info lá, deixar printado na tela
        if (debugText != null && DataPlayer.Instace != null)
        {
            // O '$' antes da string permite colocar variáveis direto no texto
            debugText.text = $"Nome: {DataPlayer.Instace.nome}\n" +
                             $"Level: {DataPlayer.Instace.level}\n" +
                             $"Ouro: {DataPlayer.Instace.ouro}\n" +
                             $"Vida: {DataPlayer.Instace.Vida}";
        }

        if (Input.GetKeyDown(KeyCode.G)) // G da GOLD DINHEIRO CASH
        {
            DataPlayer.Instace.ouro += 10;
            Debug.Log("Ganhou 10 de ouro!");
        }
    }

    private void Attack()
    {

        //Se o ponteiro do mouse (ou o toque na tela)
        //ta sobre um objeto de UI
        //
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // não faz nada e sai imediatamente da função.
            return;
        }
        //



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

    public void ResetarMovimento()
    {
        // Força a variável de input de movimento a ser zero.
        moveInput = Vector2.zero;
        // Se você tiver outras variáveis de estado (como pulando, correndo),
        // também seria bom resetá-las aqui.
        Debug.Log("Movimento do jogador resetado.");
    }
}

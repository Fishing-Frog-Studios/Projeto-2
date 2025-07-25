using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("Configurações de Ataque Ranged")]
    public GameObject projectilePrefab; // Arraste o Prefab do seu projétil aqui
    public Transform firePoint; // Ponto de onde o projétil sai. Pode ser o próprio transform do jogador.

    // Timers e Cooldowns
    private float attackCooldownTimer = 0f;
    private float ammoRegenTimer = 0f;
    private float timeStationary = 0f;

    private void Awake()
    {
        // Pega referências dos componentes
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Configura o Input System
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Ataque.performed += ctx => TryAttack();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        // Garante que o jogador não comece invisível e com a cor normal
        SetInvisibility(false);
        // Garante que o ponto de disparo exista, mesmo que seja o próprio jogador
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        // Atualiza todos os timers a cada frame
        HandleAttackCooldown();
        HandleAmmoRegen();
        HandleInvisibility();
    }

    void FixedUpdate()
    {
        // Movimentação baseada em física é mais consistente
        if (DataPlayer.Instance != null)
        {
            rb.linearVelocity = moveInput.normalized * DataPlayer.Instance.moveSpeed;
        }
    }

    private void TryAttack()
    {
        if (DataPlayer.Instance == null || attackCooldownTimer > 0) return;

        // Verifica qual modo de ataque está ativo
        if (DataPlayer.Instance.currentAttackMode == AttackMode.Standard)
        {
            // Ataque padrão com projéteis e munição
            if (DataPlayer.Instance.currentAmmo > 0)
            {
                StartCoroutine(FireStandardProjectiles());
                DataPlayer.Instance.currentAmmo--;
                attackCooldownTimer = DataPlayer.Instance.ataqueSpeed;

                // Atacar quebra a invisibilidade
                if (DataPlayer.Instance.isInvisible)
                {
                    SetInvisibility(false);
                }
            }
        }
        else // AttackMode.Orbiting
        {
            StartCoroutine(FireOrbitingProjectiles());
            attackCooldownTimer = DataPlayer.Instance.orbitDuration; // Cooldown é a própria duração da órbita
        }
    }

    private IEnumerator FireStandardProjectiles()
    {
        for (int i = 0; i < DataPlayer.Instance.projectileCount; i++)
        {
            Vector3 mousePos = DataPlayer.Instance.mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = (mousePos - firePoint.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            
            // O projétil buscará o dano no momento do impacto
            proj.GetComponent<Rigidbody2D>().linearVelocity = direction * 15f; // Ajuste a velocidade do projétil

            yield return new WaitForSeconds(0.1f); // Pequeno atraso entre projéteis múltiplos
        }
    }

    private IEnumerator FireOrbitingProjectiles()
    {
        GameObject[] orbitingProjs = new GameObject[DataPlayer.Instance.orbitingProjectilesCount];
        for (int i = 0; i < orbitingProjs.Length; i++)
        {
            orbitingProjs[i] = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            if (orbitingProjs[i].TryGetComponent<Rigidbody2D>(out var projRb))
            {
                projRb.linearVelocity = Vector2.zero; // Projéteis giratórios são controlados por posição
                projRb.bodyType = RigidbodyType2D.Kinematic; // Evita colisões estranhas
            }
        }

        float timer = 0f;
        while (timer < DataPlayer.Instance.orbitDuration)
        {
            for (int i = 0; i < orbitingProjs.Length; i++)
            {
                if(orbitingProjs[i] == null) continue;

                float angle = (i * (360f / orbitingProjs.Length)) + (timer * 360f / DataPlayer.Instance.orbitDuration);
                Vector2 offset = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * DataPlayer.Instance.orbitRadius;
                orbitingProjs[i].transform.position = (Vector2)transform.position + offset;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        
        foreach(var proj in orbitingProjs)
        {
            if (proj != null) Destroy(proj);
        }
    }
    
    // --- MÉTODOS DE GERENCIAMENTO (Chamados no Update) ---

    private void HandleAttackCooldown()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleAmmoRegen()
    {
        if (DataPlayer.Instance == null || DataPlayer.Instance.currentAmmo >= DataPlayer.Instance.maxAmmo) return;
        
        ammoRegenTimer += Time.deltaTime;
        if (ammoRegenTimer >= DataPlayer.Instance.ammoRegenRate)
        {
            DataPlayer.Instance.currentAmmo++;
            ammoRegenTimer = 0f;
        }
    }

    private void HandleInvisibility()
    {
        if (DataPlayer.Instance == null || !DataPlayer.Instance.canBecomeInvisible) return;

        if (rb.linearVelocity.sqrMagnitude > 0.01f) // Se o jogador está se movendo
        {
            timeStationary = 0f;
            if (DataPlayer.Instance.isInvisible)
            {
                SetInvisibility(false);
            }
        }
        else // Se o jogador está parado
        {
            timeStationary += Time.deltaTime;
            if (timeStationary >= DataPlayer.Instance.timeToBecomeInvisible && !DataPlayer.Instance.isInvisible)
            {
                SetInvisibility(true);
                DataPlayer.Instance.nextAttackHasBonusDamage = true;
            }
        }
    }

    // --- MÉTODOS AUXILIARES ---

    private void SetInvisibility(bool state)
    {
        if (DataPlayer.Instance == null) return;
        
        DataPlayer.Instance.isInvisible = state;
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = state ? 0.5f : 1.0f; // Opacidade para feedback visual
        spriteRenderer.color = spriteColor;
        
        if (!state)
        {
             DataPlayer.Instance.nextAttackHasBonusDamage = false;
        }
    }

    public float CalculateFinalDamage()
    {
        if (DataPlayer.Instance == null) return 0f;

        float finalDamage = DataPlayer.Instance.danoBase;
        if (DataPlayer.Instance.nextAttackHasBonusDamage)
        {
            finalDamage *= 2;
            DataPlayer.Instance.nextAttackHasBonusDamage = false; // Bônus é consumido no ataque
        }
        return finalDamage;
    }

    // Removemos OnDrawGizmosSelected pois o ataque melee não existe mais.
}

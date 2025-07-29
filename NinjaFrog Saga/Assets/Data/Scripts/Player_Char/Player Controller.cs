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
    public GameObject projectilePrefab;
    public Transform firePoint;

    // Timers e Cooldowns
    private float attackCooldownTimer = 0f;
    private float ammoRegenTimer = 0f;
    private float timeStationary = 0f;
    
    // <<< NOVO: Status Ativos para evitar buffs cumulativos >>>
    private float activeDamage;
    private float activeAttackSpeed;
    private bool isBuffActive = false;

    private Coroutine postInvisibilityBuffCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Ataque.performed += ctx => TryAttack();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        SetInvisibility(false, true);
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        // <<< NOVO: Atualiza os status ativos a cada frame >>>
        // Se o buff não estiver ativo, os status ativos são iguais aos do DataPlayer
        if (!isBuffActive)
        {
            activeDamage = DataPlayer.Instance.danoBase;
            activeAttackSpeed = DataPlayer.Instance.ataqueSpeed;
        }

        HandleAttackCooldown();
        HandleAmmoRegen();
        HandleInvisibility();
    }

    void FixedUpdate()
    {
        if (DataPlayer.Instance != null)
        {
            rb.linearVelocity = moveInput.normalized * DataPlayer.Instance.moveSpeed;
        }
    }

    private void TryAttack()
    {
        if (DataPlayer.Instance == null || attackCooldownTimer > 0) return;

        if (DataPlayer.Instance.currentAttackMode == AttackMode.Standard)
        {
            if (DataPlayer.Instance.currentAmmo > 0)
            {
                if (DataPlayer.Instance.isInvisible)
                {
                    SetInvisibility(false);
                }
                
                StartCoroutine(FireStandardProjectiles());
                DataPlayer.Instance.currentAmmo--;
                
                // <<< CORRIGIDO: Usa o 'activeAttackSpeed' para o cooldown >>>
                attackCooldownTimer = activeAttackSpeed;
            }
        }
        else
        {
            StartCoroutine(FireOrbitingProjectiles());
            attackCooldownTimer = DataPlayer.Instance.orbitDuration;
        }
    }

    private IEnumerator FireStandardProjectiles()
    {
        for (int i = 0; i < DataPlayer.Instance.projectileCount; i++)
        {
            Vector3 mousePos = DataPlayer.Instance.mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = (mousePos - firePoint.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            
            // <<< CORRIGIDO: O dano do projétil é calculado com 'activeDamage' >>>
            float finalDamage = CalculateFinalDamage();
            // Lembre-se de passar 'finalDamage' para o script do seu projétil
            // Ex: proj.GetComponent<Projectile>().Initialize(direction, finalDamage);

            proj.GetComponent<Rigidbody2D>().linearVelocity = direction * 15f; 

            if (DataPlayer.Instance.projectileCount > 1)
                yield return new WaitForSeconds(0.1f);
        }
    }

    // O restante do script continua igual até o método SetInvisibility...
    // ... (FireOrbitingProjectiles, HandleAttackCooldown, etc. não precisam de mudança) ...

    private void HandleInvisibility()
    {
        if (DataPlayer.Instance == null || !DataPlayer.Instance.canBecomeInvisible) return;

        if (rb.linearVelocity.sqrMagnitude > 0.01f) // Se está se movendo
        {
            timeStationary = 0f;
            if (DataPlayer.Instance.isInvisible)
            {
                SetInvisibility(false);
            }
        }
        else // Se está parado
        {
            timeStationary += Time.deltaTime;
            if (timeStationary >= DataPlayer.Instance.timeToBecomeInvisible && !DataPlayer.Instance.isInvisible)
            {
                SetInvisibility(true);
            }
        }
    }
    
    private void SetInvisibility(bool state, bool forceState = false)
    {
        if (DataPlayer.Instance == null) return;
        if (DataPlayer.Instance.isInvisible == state && !forceState) return;
        
        bool wasInvisible = DataPlayer.Instance.isInvisible;
        DataPlayer.Instance.isInvisible = state;

        Color spriteColor = spriteRenderer.color;
        spriteColor.a = state ? 0.5f : 1.0f;
        spriteRenderer.color = spriteColor;
        
        if (state)
        {
            DataPlayer.Instance.nextAttackHasBonusDamage = true;
        }
        else if (wasInvisible)
        {
            if (DataPlayer.Instance.hasPostInvisibilityBuffSkill)
            {
                if (postInvisibilityBuffCoroutine != null)
                {
                    StopCoroutine(postInvisibilityBuffCoroutine);
                }
                postInvisibilityBuffCoroutine = StartCoroutine(PostInvisibilityBuffRoutine());
            }
        }
    }

    // <<< CORRIGIDO: Coroutine agora usa os status ativos >>>
    private IEnumerator PostInvisibilityBuffRoutine()
    {
        isBuffActive = true;
        Debug.Log("SKILL 8 ATIVADA: +20% Dano, +Velocidade de Ataque por 5s!");

        // Aplica os buffs calculados a partir dos valores BASE do DataPlayer
        activeDamage = DataPlayer.Instance.danoBase * 1.20f;
        activeAttackSpeed = DataPlayer.Instance.ataqueSpeed * 0.8f; // 20% de redução no cooldown = 25% mais rápido
        
        yield return new WaitForSeconds(5f);

        isBuffActive = false;
        postInvisibilityBuffCoroutine = null;
        Debug.Log("SKILL 8 TERMINOU: Status voltando ao normal.");
    }

    public float CalculateFinalDamage()
    {
        if (DataPlayer.Instance == null) return 0f;

        // Começa com o dano ativo (que já pode estar bufado pela Skill 8)
        float finalDamage = activeDamage;
        
        if (DataPlayer.Instance.nextAttackHasBonusDamage)
        {
            finalDamage *= 1.5f;
            DataPlayer.Instance.nextAttackHasBonusDamage = false;
        }
        return finalDamage;
    }
    
    // O resto do código (como FireOrbitingProjectiles) não precisa de alteração.
    private IEnumerator FireOrbitingProjectiles()
    {
        GameObject[] orbitingProjs = new GameObject[DataPlayer.Instance.orbitingProjectilesCount];
        for (int i = 0; i < orbitingProjs.Length; i++)
        {
            orbitingProjs[i] = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            if (orbitingProjs[i].TryGetComponent<Rigidbody2D>(out var projRb))
            {
                projRb.linearVelocity = Vector2.zero;
                projRb.bodyType = RigidbodyType2D.Kinematic;
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
}

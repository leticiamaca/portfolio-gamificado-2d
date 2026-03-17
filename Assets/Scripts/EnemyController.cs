using System.Collections;
using UnityEngine;

/// <summary>
/// Sistema completo de IA do inimigo:
/// - Perseguição com detecção por distância
/// - Ataque normal (combo de 3)
/// - Ataque especial após 3 ataques normais (com cooldown prolongado)
/// - Sistema de dano e morte
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))] // Troque por Rigidbody se for 3D
public class EnemyController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // CONFIGURAÇÕES GERAIS
    // ─────────────────────────────────────────────
    [Header("Referências")]
    [SerializeField] private Transform player;              // Arraste o Player aqui no Inspector
    private Animator anim;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 3f;          // Velocidade de caminhada
    [SerializeField] private float chaseRange = 8f;         // Distância para começar a perseguir
    [SerializeField] private float attackRange = 1.5f;      // Distância para atacar

    [Header("Ataque")]
    [SerializeField] private float attackCooldown = 0.6f;    // Tempo entre ataques normais
    [SerializeField] private float specialAttackCooldown = 2f; // Cooldown APÓS o ataque especial
    [SerializeField] private int attacksBeforeSpecial = 3;   // Quantos ataques antes do especial
    [SerializeField] private int attackDamage = 10;          // Dano ataque normal
    [SerializeField] private int specialAttackDamage = 25;   // Dano ataque especial
    [SerializeField] private float attackHitboxDelay = 0.3f; // Delay até o hitbox do ataque ativo

    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;

    // ─────────────────────────────────────────────
    // VARIÁVEIS INTERNAS
    // ─────────────────────────────────────────────
    private int currentHealth;
    private int attackComboCount = 0;
    private float attackTimer = 0f;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool isInCooldownAfterSpecial = false;

    // Hash dos parâmetros da Animator (mais performático que string)
    private static readonly int AnimIsWalking = Animator.StringToHash("isWalking");
    private static readonly int AnimAttack = Animator.StringToHash("Attack");
    private static readonly int AnimSpecialAttack = Animator.StringToHash("SpecialAttack");
    private static readonly int AnimTakeDamage = Animator.StringToHash("TakeDamage");
    private static readonly int AnimDie = Animator.StringToHash("Die");

    // ─────────────────────────────────────────────
    // INICIALIZAÇÃO
    // ─────────────────────────────────────────────
    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Busca automática do player se não estiver setado no Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("[EnemyController] Player não encontrado! Adicione a tag 'Player' ou arraste no Inspector.");
        }
    }

    // ─────────────────────────────────────────────
    // UPDATE — MÁQUINA DE ESTADOS PRINCIPAL
    // ─────────────────────────────────────────────
    private void Update()
    {
        // Morto: não faz nada
        if (isDead) return;
        // Atacando: aguarda animação acabar
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position); // Use Vector3 se 3D

        attackTimer -= Time.deltaTime;

        // ── ESTADO: ATAQUE ──────────────────────────────
        if (distanceToPlayer <= attackRange && attackTimer <= 0f && !isInCooldownAfterSpecial)
        {
            attackComboCount++;

            if (attackComboCount > attacksBeforeSpecial)
            {
                // Resetamos ANTES de iniciar para evitar dupla contagem
                attackComboCount = 0;
                StartCoroutine(PerformSpecialAttack());
            }
            else
            {
                StartCoroutine(PerformNormalAttack());
            }

            return;
        }

        // ── ESTADO: PERSEGUIÇÃO ─────────────────────────
        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        // ── ESTADO: IDLE ────────────────────────────────
        else if (distanceToPlayer > chaseRange)
        {
            Idle();
        }
        // ── AGUARDANDO NO RANGE (cooldown ativo) ─────────
        else
        {
            anim.SetBool(AnimIsWalking, false);
        }
    }

    // ─────────────────────────────────────────────
    // MOVIMENTO
    // ─────────────────────────────────────────────
    private void ChasePlayer()
    {
        anim.SetBool(AnimIsWalking, true);

        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // Flip horizontal (para jogos 2D)
        if (direction.x != 0)
            transform.localScale = new Vector3(
                Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
    }

    private void Idle()
    {
        anim.SetBool(AnimIsWalking, false);
    }

    // ─────────────────────────────────────────────
    // ATAQUE NORMAL
    // ─────────────────────────────────────────────
    private IEnumerator PerformNormalAttack()
    {
        isAttacking = true;
        anim.SetBool(AnimIsWalking, false);
        anim.SetTrigger(AnimAttack);

        // Aguarda o momento do hit (sincroniza com a animação)
        yield return new WaitForSeconds(attackHitboxDelay);
        //ApplyDamageToPlayer(attackDamage);

        // Aguarda o restante do cooldown antes de liberar o próximo ataque
        yield return new WaitForSeconds(attackCooldown - attackHitboxDelay);

        attackTimer = 0f;
        isAttacking = false;
    }

    // ─────────────────────────────────────────────
    // ATAQUE ESPECIAL
    // ─────────────────────────────────────────────
    private IEnumerator PerformSpecialAttack()
    {
        isAttacking = true;
        isInCooldownAfterSpecial = true;

        anim.SetBool(AnimIsWalking, false);
        anim.SetTrigger(AnimSpecialAttack);

        // Hitbox aparece um pouco depois (animação de especial costuma ter wind-up)
        yield return new WaitForSeconds(attackHitboxDelay * 1.5f);
        //ApplyDamageToPlayer(specialAttackDamage);

        // Cooldown prolongado — a animação é mais longa, evitamos bugs
        yield return new WaitForSeconds(specialAttackCooldown);

        isAttacking = false;
        isInCooldownAfterSpecial = false;
        attackTimer = 0f;
    }

    // ─────────────────────────────────────────────
    // APLICA DANO NO PLAYER
    // ─────────────────────────────────────────────
    //private void ApplyDamageToPlayer(int damage)
    //{
    //    // Verifica se o player ainda está no range no momento do hit
    //    if (player == null) return;

    //    float dist = Vector2.Distance(transform.position, player.position);
    //    if (dist > attackRange * 1.3f) return; // Margem de tolerância

    //    // Tenta chamar o método de dano do player
    //    // Seu script de player deve ter um método "TakeDamage(int)"
    //    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    //    if (playerHealth != null)
    //    {
    //        playerHealth.TakeDamage(damage);
    //    }
    //    else
    //    {
    //        // Fallback: manda via SendMessage (mais fácil de integrar)
    //        player.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    //    }
    //}

    // ─────────────────────────────────────────────
    // SISTEMA DE DANO (inimigo recebendo dano)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Chamado externamente (ex: pelo script da espada do player)
    /// Exemplo: enemy.GetComponent<EnemyController>().TakeDamage(20);
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[Enemy] Tomou {damage} de dano. HP restante: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Interrompe ação atual para tocar hit reaction
            StopAllCoroutines();
            isAttacking = false;
            isInCooldownAfterSpecial = false;
            StartCoroutine(HitReaction());
        }
    }

    private IEnumerator HitReaction()
    {
        isAttacking = true; // Trava movimento durante a reação
        anim.SetTrigger(AnimTakeDamage);
        yield return new WaitForSeconds(0.4f); // Duração da animação de hit
        isAttacking = false;

        // Reinicia o cooldown do ataque especial para evitar combo interrompido bugado
        attackTimer = attackCooldown;
    }

    // ─────────────────────────────────────────────
    // MORTE
    // ─────────────────────────────────────────────
    private void Die()
    {
        isDead = true;
        StopAllCoroutines();

        anim.SetBool(AnimIsWalking, false);
        anim.SetTrigger(AnimDie);

        // Desativa colliders para não bloquear mais o player
        Collider2D col = GetComponent<Collider2D>(); // Use Collider se for 3D
        if (col != null) col.enabled = false;

        // Destrói o objeto após a animação de morte terminar
        StartCoroutine(DestroyAfterDeath());

        Debug.Log("[Enemy] Morreu.");
    }

    private IEnumerator DestroyAfterDeath()
    {
        // Aguarda a animação de morte (ajuste o tempo conforme a duração da sua animação)
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
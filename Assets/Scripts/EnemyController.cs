using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;
    private Animator anim;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Ataque")]
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private float specialAttackCooldown = 2f;
    [SerializeField] private int attacksBeforeSpecial = 3;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int specialAttackDamage = 25;
    [SerializeField] private float attackHitboxDelay = 0.3f;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private int attackComboCount = 0;
    private float attackTimer = 0f;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool isInCooldownAfterSpecial = false;

    private static readonly int AnimIsWalking    = Animator.StringToHash("isWalking");
    private static readonly int AnimAttack       = Animator.StringToHash("Attack");
    private static readonly int AnimSpecialAttack = Animator.StringToHash("SpecialAttack");
    private static readonly int AnimTakeDamage   = Animator.StringToHash("Hurt");
    private static readonly int AnimDie          = Animator.StringToHash("Die");


    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("[EnemyController] Player não encontrado!");
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (player == null || !player.gameObject.activeInHierarchy) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        FacePlayer();

        if (isAttacking) return;
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackRange && attackTimer <= 0f && !isInCooldownAfterSpecial)
        {
            attackComboCount++;

            if (attackComboCount > attacksBeforeSpecial)
            {
                attackComboCount = 0;
                StartCoroutine(PerformSpecialAttack());
            }
            else
            {
                StartCoroutine(PerformNormalAttack());
            }

            return;
        }

        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
            ChasePlayer();
        else if (distanceToPlayer > chaseRange)
            Idle();
        else
            anim.SetBool(AnimIsWalking, false);
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        anim.SetBool(AnimIsWalking, true);
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void FacePlayer()
    {
        if (player == null) return;
        float direction = player.position.x - transform.position.x;
        if (direction != 0)
            transform.localScale = new Vector3(
                -Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x),
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

        yield return new WaitForSeconds(attackHitboxDelay);
        ApplyDamageToPlayer(attackDamage);

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

        yield return new WaitForSeconds(attackHitboxDelay * 1.5f);
        ApplyDamageToPlayer(specialAttackDamage);

        yield return new WaitForSeconds(specialAttackCooldown);
        isAttacking = false;
        isInCooldownAfterSpecial = false;
        attackTimer = 0f;
    }

    // ─────────────────────────────────────────────
    // APLICAR DANO NO PLAYER
    // ─────────────────────────────────────────────
    private void ApplyDamageToPlayer(int damage)
    {
        if (player == null) return;

        // verifica se o player ainda está perto no momento do hit
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > attackRange * 1.3f) return;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.TakeDamage(damage);
    }

    // ─────────────────────────────────────────────
    // TOMAR DANO
    // ─────────────────────────────────────────────
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[Enemy] Tomou {damage} de dano. HP restante: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
        else
        {
            StopAllCoroutines();
            isAttacking = false;
            isInCooldownAfterSpecial = false;
            StartCoroutine(HitReaction());
        }
    }

    private IEnumerator HitReaction()
    {
        isAttacking = true;
        anim.SetTrigger(AnimTakeDamage);
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
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

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(DestroyAfterDeath());
        Debug.Log("[Enemy] Morreu.");
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
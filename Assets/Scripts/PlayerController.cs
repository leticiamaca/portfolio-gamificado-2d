using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Unity.Cinemachine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Ataque")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackHitboxDelay = 0.2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityTime = 1f;

    [SerializeField] private Slider healthSlider;

    private Animator anim;
    private int currentHealth;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isDead = false;
    private float invincibilityTimer = 0f;

    private Vector2 lastDirection = Vector2.down;
    private bool isHurt = false;

    private Vector3 savedPosition;
    private bool hasSavedPosition = false;

    private CinemachineCamera virtualCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0f;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Restaura posição salva
        if (hasSavedPosition)
        {
            transform.position = savedPosition;
            hasSavedPosition = false;
        }

        // Re-atribui o Cinemachine da nova cena ao player
        virtualCamera = Object.FindFirstObjectByType<CinemachineCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        // Re-atribui o Slider da nova cena
        healthSlider = Object.FindFirstObjectByType<Slider>();
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SavePosition()
    {
        savedPosition = transform.position;
        hasSavedPosition = true;
    }

    private void OnEnable()
    {
        if (hasSavedPosition)
            transform.position = savedPosition;
    }

    private void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;

        HandleMovement();
        HandleAttack();
    }

    private void HandleMovement()
    {
        if (isAttacking) return;
        if (isHurt) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 && v != 0) { anim.Play("Idle"); return; }

        if (v < 0) { lastDirection = Vector2.down; anim.Play("Front-walk"); }
        else if (v > 0) { lastDirection = Vector2.up; anim.Play("Back-walk"); }
        else if (h < 0) { lastDirection = Vector2.left; anim.Play("Left-walk"); }
        else if (h > 0) { lastDirection = Vector2.right; anim.Play("Right-walk"); }
        else { anim.Play("Idle"); }

        transform.Translate(new Vector2(h, v) * moveSpeed * Time.deltaTime);
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && attackTimer <= 0f && !isAttacking)
            StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        if (lastDirection == Vector2.left) anim.Play("Left-atack");
        else anim.Play("Right-atack");

        yield return new WaitForSeconds(attackHitboxDelay);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
                enemy.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown - attackHitboxDelay);
        isAttacking = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;
        if (invincibilityTimer > 0f) return;

        if (other.CompareTag("Enemy"))
            TakeDamage(10);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (invincibilityTimer > 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        invincibilityTimer = invincibilityTime;
        isAttacking = false;

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(HurtReaction());
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    private IEnumerator HurtReaction()
    {
        isHurt = true;
        anim.Play("Damage");
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return null;
        gameObject.SetActive(false);
        FadeSystem.Instance.LoadScene("GameOver");
    }
}
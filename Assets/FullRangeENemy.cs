using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullRangeENemy : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;
    public float bounceForce = 5f;
    public float flashDuration = 0.1f;
    public float knockbackDuration = 0.5f;
    public float knockbackForce = 10f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;
    public float attackRange = 10f;
    public float detectionRange = 15f;
    public float projectileSpeed = 10f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float attackTimer = 0f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private bool isDead = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Transform player;

    private Color originalColor;
    private RangeAttack rangeAttack;
    private EnemyMovement enemyMovement;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rangeAttack = GetComponent<RangeAttack>();
        enemyMovement = GetComponent<EnemyMovement>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found!");
        }
    }

    void Update()
    {
        if (isDead)
            return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; // Cooldown timer
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // If player is within detection range
            if (distanceToPlayer <= detectionRange)
            {
                // If player is within attack range, attack
                if (distanceToPlayer <= attackRange && attackTimer <= 0)
                {
                    Attack();
                }

                // Handle movement
                if (distanceToPlayer > attackRange)
                {
                    MoveTowardsPlayer();
                }
                else
                {
                    StopMovement();
                }

                // Flip sprite even when stationary
                FlipSprite((player.position - transform.position).normalized);
            }
            else
            {
                ResumeMovement();
            }
        }

        HandleKnockback();
    }

    private void Attack()
    {
        attackTimer = attackCooldown; // Start cooldown
        animator.SetBool("isAttacking", true); // Trigger attack animation
        FireProjectile(); // Fire a projectile

        StartCoroutine(ResetAttackAnimation());
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        Debug.Log("Fired projectile at player");
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Adjust according to your attack animation
        animator.SetBool("isAttacking", false);
    }

    public void TakeDamage(int damage, Vector2 damageSource)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (spriteRenderer != null)
        {
            StopCoroutine("FlashRed"); // Stop any existing flash effect
            StartCoroutine(FlashRed()); // Flash on damage
        }

        if (rb != null && !isKnockedBack)
        {
            ApplyKnockback(damageSource);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void HandleKnockback()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void ApplyKnockback(Vector2 damageSource)
    {
        Vector2 knockbackDirection = (transform.position - (Vector3)damageSource).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        animator.SetBool("isMoving", direction.magnitude > 0f);
        FlipSprite(direction);
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x < 0f && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0f && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void StopMovement()
    {
        if (enemyMovement != null)
        {
            enemyMovement.enabled = false;  // Stop movement when in attack range
        }
    }

    private void ResumeMovement()
    {
        if (enemyMovement != null)
        {
            enemyMovement.enabled = true;  // Resume movement when outside attack range
        }
    }

    private void Die()
    {
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject, 5f); // Delay destruction to allow death animation
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

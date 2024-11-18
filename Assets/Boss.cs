using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public int maxHealth = 300;
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

    // Health bar UI
    public Slider healthBarSlider;

    private float attackTimer = 0f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private bool isDead = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Transform player;
    private Color originalColor;

    [System.Serializable]
    public class SpawnEvent
    {
        public GameObject objectToSpawn;
        public Transform spawnLocation; // Location to spawn the object
        public float healthThreshold; // Percentage (e.g., 0.5 for 50% health)
        public bool hasSpawned = false; // Tracks if the object has already been spawned
    }

    public List<SpawnEvent> spawnEvents; // Set this up in the Inspector

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found!");
        }

        // Initialize health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        // Handle attack timer cooldown
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // If player is within detection range
            if (distanceToPlayer <= detectionRange)
            {
                // If within attack range and attack is off cooldown, attack
                if (distanceToPlayer <= attackRange && attackTimer <= 0)
                {
                    Attack();
                }
                // Handle movement
                else if (distanceToPlayer > attackRange)
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
                StopMovement();
            }
        }

        HandleKnockback();
    }

    private void Attack()
    {
        attackTimer = attackCooldown;  // Start cooldown
        animator.SetBool("isAttacking", true);  // Trigger attack animation
        FireProjectile();  // Fire a projectile
        StartCoroutine(ResetAttackAnimation());
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.velocity = direction * projectileSpeed;

        Debug.Log("Fired projectile at player");
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f);  // Adjust according to your attack animation
        animator.SetBool("isAttacking", false);
    }

    public void TakeDamage(int damage, Vector2 damageSource)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Update health bar UI
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        if (spriteRenderer != null)
        {
            StopCoroutine("FlashRed");  // Stop any existing flash effect
            StartCoroutine(FlashRed());  // Flash on damage
        }

        if (rb != null && !isKnockedBack)
            ApplyKnockback(damageSource);

        // Check each spawn event
        foreach (SpawnEvent spawnEvent in spawnEvents)
        {
            if (!spawnEvent.hasSpawned && currentHealth <= maxHealth * spawnEvent.healthThreshold)
            {
                spawnEvent.hasSpawned = true;
                SpawnObject(spawnEvent.objectToSpawn, spawnEvent.spawnLocation);
            }
        }

        if (currentHealth <= 0)
            Die();
    }

    private void SpawnObject(GameObject objectToSpawn, Transform spawnLocation)
    {
        if (objectToSpawn != null)
        {
            Vector3 position = spawnLocation != null ? spawnLocation.position : transform.position;
            Instantiate(objectToSpawn, position, Quaternion.identity); // Spawn at specified location or at the boss position
            Debug.Log($"{objectToSpawn.name} spawned at {position}!");
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

    private void StopMovement()
    {
        animator.SetBool("isMoving", false);
    }

    private void FlipSprite(Vector2 direction)
    {
        if ((direction.x < 0f && !spriteRenderer.flipX) || (direction.x > 0f && spriteRenderer.flipX))
            spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void Die()
    {
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");
        Debug.Log($"{gameObject.name} has died.");

        // Optionally hide health bar when the boss dies
        if (healthBarSlider != null)
            healthBarSlider.gameObject.SetActive(false);

        Destroy(gameObject, 5f);  // Delay destruction to allow death animation
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public WaveManager waveManager;
    public int maxHealth = 50;
    public int currentHealth;
    public float bounceForce = 5f; // Force of bounce-back after damage
    public float flashDuration = 0.1f; // Duration of flash effect after taking damage
    public float knockbackDuration = 0.5f; // Knockback duration
    public float knockbackForce = 10f; // Force applied during knockback

    private SpriteRenderer spriteRenderer;
    private Color originalColor; // To revert to original color after flash
    private Rigidbody2D rb;
    private Animator animator;

    private bool isKnockedBack = false; // Track if knockback is happening
    private float knockbackTimer = 0f;

    public EnemyMovement enemyMovement;
    void Start()
    {
        waveManager = GetComponent<WaveManager>();
        currentHealth = maxHealth; // Initialize health
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null) originalColor = spriteRenderer.color; // Store original color for flashing effect
        if (rb == null) Debug.LogWarning($"{gameObject.name} is missing Rigidbody2D.");
        if (animator == null) Debug.LogWarning($"{gameObject.name} is missing Animator.");
    }

    void Update()
    {
        HandleKnockback();
    }

    // Apply damage and manage health
    public void TakeDamage(int damage, Vector2 damageSource)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        if (spriteRenderer != null) StartCoroutine(FlashRed()); // Flash red on damage

        if (rb != null && !isKnockedBack)
        {
            // Apply knockback if the enemy is not already knocked back
            ApplyKnockback(damageSource);
        }

        if (currentHealth <= 0) Die();
    }

    private void HandleKnockback()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime; // Reduce knockback timer
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false; // End knockback effect
                rb.velocity = Vector2.zero; // Stop any ongoing knockback velocity
            }
        }
    }

    private void ApplyKnockback(Vector2 damageSource)
    {
        Vector2 knockbackDirection = (transform.position - (Vector3)damageSource).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        isKnockedBack = true;
        knockbackTimer = knockbackDuration; // Set knockback timer
    }

    // Flash the sprite red when the enemy takes damage
    private System.Collections.IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // Handle enemy death
    private void Die()
    {
        waveManager.OnEnemyDefeated();
        // enemyMovement.enabled=false;
        if (animator != null) animator.SetTrigger("Die");
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject, 4f); // Destroy enemy after death animation (1 second delay)
    }

    IEnumerator DieANimations()
    {
        Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        Debug.Log("End Coroutine");
    }

}

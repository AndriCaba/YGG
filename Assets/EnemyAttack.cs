using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyAttack : MonoBehaviour
{
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public float attackRange = 3f; // Range at which the enemy attacks
    public float detectionRange = 10f; // Range at which the enemy detects the player
    public float damageDelay = 0.3f; // Delay before dealing damage

    public Transform player;
    private float attackTimer = 0f;
    private Animator animator;

    // Knockback variables
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    // Red flash variables
    private Renderer enemyRenderer;
    public Color flashColor = Color.red; // Red flash color
    public float flashDuration = 0.1f;

    // Audio variables
    public AudioClip attackSound;
    private AudioSource audioSource;

    // Movement variables
    private EnemyMovement enemyMovement;  // Reference to the enemy movement script

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        enemyRenderer = GetComponent<Renderer>(); // Get Renderer for color flash
        audioSource = GetComponent<AudioSource>(); // Get AudioSource for attack sound

        // Get the movement script (assuming you have an EnemyMovement script attached to the same object)
        enemyMovement = GetComponent<EnemyMovement>();

        // Try to find the player automatically by tag or name
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            // Try to find player by name if not found by tag
            player = GameObject.Find("Player")?.transform;
        }

        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure there is a GameObject with the 'Player' tag or 'Player' name.");
        }
    }

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; // Cooldown timer
        }

        // Check if the player is within detection range
        if (player != null && IsPlayerInRange())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // If player is in attack range, attack
            if (distanceToPlayer <= attackRange && attackTimer <= 0)
            {
                Attack();
            }

            // Stop enemy movement if within attack range
            if (distanceToPlayer <= attackRange)
            {
                StopMovement();  // Stop movement when in attack range
            }
            else
            {
                ResumeMovement();  // Resume movement when outside of attack range
            }
        }
        else
        {
            CheckPlayerInRange();
            ResumeMovement();  // Always resume movement if no player is in range
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private void CheckPlayerInRange()
    {
        // Use a Physics2D check to detect the player in range
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            player = playerCollider.transform;
        }
        else
        {
            player = null;
        }
    }

    private void Attack()
    {
        attackTimer = attackCooldown; // Start cooldown

        // Trigger the attack animation
        animator.SetBool("isAttacking", true);

        // Play attack sound
        PlayAttackSound();

        // Start the damage delay coroutine
        StartCoroutine(DealDamageWithDelay());

        // Flash red after attacking
        StartCoroutine(FlashRed());

        // Reset the attack animation after a short duration (depending on your attack animation)
        StartCoroutine(ResetAttackAnimation());
    }

    public IEnumerator DealDamageWithDelay()
    {
        // Wait for the damage delay before dealing damage
        yield return new WaitForSeconds(damageDelay);

        // Deal damage to the player if they are in range
        PlayerHealth playerHealth = player?.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Calculate the knockback direction (away from the enemy)
            Vector2 knockbackDirection = (player.position - transform.position).normalized * -1;

            // Pass the knockback direction along with the damage
            playerHealth.TakeDamage(attackDamage, knockbackDirection);
            Debug.Log($"{gameObject.name} attacked the player for {attackDamage} damage.");

            // Apply knockback to the player
            ApplyKnockback(knockbackDirection);
        }
    }

    public bool IsPlayerInAttackRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    private void ApplyKnockback(Vector2 knockbackDirection)
    {
        // Apply knockback to the player
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero; // Reset player's velocity
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    private IEnumerator FlashRed()
    {
        if (enemyRenderer != null)
        {
            Color originalColor = enemyRenderer.material.color;
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                enemyRenderer.material.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(elapsed * 5, 1));
                elapsed += Time.deltaTime;
                yield return null;
            }
            enemyRenderer.material.color = originalColor; // Restore original color
        }
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Wait time should match the length of the attack animation
        animator.SetBool("isAttacking", false);
    }

    private void StopMovement()
    {
        if (enemyMovement != null)
        {
            enemyMovement.enabled = false;  // Disable the movement script to stop the enemy
        }
    }

    private void ResumeMovement()
    {
        if (enemyMovement != null)
        {
            enemyMovement.enabled = true;   // Enable the movement script to resume movement
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection range and attack range for visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

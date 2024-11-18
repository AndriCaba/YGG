using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : MonoBehaviour
{
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public float attackRange = 10f; // Range at which the enemy attacks
    public float detectionRange = 15f; // Range at which the enemy detects the player
    public float damageDelay = 0.3f; // Delay before dealing damage

    public Transform player;
    public GameObject projectilePrefab;  // Reference to the projectile prefab
    public Transform firePoint;  // Point from where the projectile is fired
    public float projectileSpeed = 10f;  // Speed of the projectile

    private float attackTimer = 0f;
    private Animator animator;
    public EnemyMovement enemyMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure there is a GameObject with the 'Player' tag.");
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
            ResumeMovement();  // Always resume movement if no player is in range
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private void Attack()
    {
        attackTimer = attackCooldown; // Start cooldown

        // Trigger attack animation
        animator.SetBool("isAttacking", true);

        // Fire a projectile
        FireProjectile();

        // Reset the attack animation after a short duration (depending on your attack animation)
        StartCoroutine(ResetAttackAnimation());
    }

    private void FireProjectile()
    {
        // Instantiate the projectile at the firePoint
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Get direction to the player
        Vector2 direction = (player.position - firePoint.position).normalized;

        // Add force to the projectile to make it move towards the player
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        Debug.Log("Fired projectile at player");
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Match the length of your attack animation
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

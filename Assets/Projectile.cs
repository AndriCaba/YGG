using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject hitEffectPrefab; // Reference to the hit effect prefab
    public int damage = 10; // Damage dealt by the projectile
    public float lifespan = 5f; // Lifespan of the projectile before it self-destructs
    public float moveSpeed = 5f; // Speed at which the projectile moves

    [Header("Effects Settings")]
    public float hitEffectDestroyDelay = 2f; // Time to destroy the hit effect
    public float knockbackForce = 5f; // Force of knockback when hitting enemies
    public float hitDelay = 0f; // Delay before applying damage

    [Header("Tracking Settings")]
    public float trackingRange = 10f; // Range within which the projectile searches for enemies
    public LayerMask enemyLayer; // Layer mask to specify which objects to track as enemies

    private Transform targetTransform; // Reference to the target's transform (enemy or boss)

    void Start()
    {
        // Automatically destroy the projectile after its lifespan expires
        Destroy(gameObject, lifespan);

        // Find the closest enemy within the tracking range
        FindClosestEnemy();
    }

    void Update()
    {
        // Move the projectile toward the target if one exists
        if (targetTransform != null)
        {
            Vector2 direction = (targetTransform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Instantiate the hit effect if a prefab is provided
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, hitEffectDestroyDelay); // Destroy the effect after a delay
        }

        // Apply damage if the collided object is an enemy or boss
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            StartCoroutine(DelayedDamage(collision.gameObject));
        }

        // Destroy the projectile after collision
        Destroy(gameObject);
    }

    // Apply damage to the target with an optional delay
    private IEnumerator DelayedDamage(GameObject target)
    {
        if (hitDelay > 0)
        {
            yield return new WaitForSeconds(hitDelay); // Wait for the specified delay
        }

        if (target.CompareTag("Boss"))
        {
            Boss boss = target.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage, transform.position);
            }
        }
        else if (target.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, transform.position);

                // Apply knockback to the enemy
                Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 knockbackDirection = (target.transform.position - transform.position).normalized;
                    ApplyKnockback(rb, knockbackDirection);
                }
            }
        }
    }

    // Apply knockback force to the target
    private void ApplyKnockback(Rigidbody2D targetRb, Vector2 knockbackDirection)
    {
        if (targetRb != null)
        {
            targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    // Find the closest enemy within the specified layer and range
    private void FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, trackingRange, enemyLayer);
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetTransform = enemy.transform;
            }
        }
    }

    // Visualize the tracking range for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
}

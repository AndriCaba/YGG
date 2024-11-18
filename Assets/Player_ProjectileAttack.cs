using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ProjectileAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab for the projectile
    public Transform projectileSpawnPoint; // Point where the projectile is spawned
    public float projectileSpeed = 10f; // Speed of the projectile

    private Animator animator;
    private Player_Movement movementScript; // Reference to Player_Movement script
    private SpriteRenderer spriteRenderer; // Reference to the player's SpriteRenderer to check facing direction

    void Start()
    {
        animator = GetComponent<Animator>(); // Reference to the Animator component
        movementScript = GetComponent<Player_Movement>(); // Reference to the movement script
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference to the SpriteRenderer component
    }

    void Update()
    {
        // Check for projectile input (right mouse click)
        if (Input.GetMouseButtonDown(1))
        {
            StartProjectileAttack(); // Start projectile attack
        }
    }

    // Start the projectile attack by playing an animation
    void StartProjectileAttack()
    {
        animator.SetTrigger("projectile"); // Trigger the projectile animation
        movementScript.StopMovement(); // Stop movement during the attack
        movementScript.StartFiringProjectile(); // Consume stamina while firing
        SpawnProjectile(); // Spawn the projectile
    }

    // Spawn the projectile and set its direction based on the player's facing
    public void SpawnProjectile()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogWarning("Projectile prefab or spawn point is not assigned!");
            return;
        }

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Determine the direction based on the player's facing direction
            Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right; // Flip direction based on flipX
            rb.velocity = direction * projectileSpeed; // Set the velocity of the projectile
        }

        movementScript.ResumeMovement(); // Resume movement after spawning the projectile
        movementScript.StopFiringProjectile(); // Allow stamina regen after firing
    }
}

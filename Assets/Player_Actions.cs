using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Actions : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public GameObject projectilePrefab;
    public GameObject hitEffectPrefab; // Projectile prefab
    public float projectileSpeed = 10f; // Speed of the projectile
    public float fireRate = 0.5f; // Time between shots
    private float lastShotTime; // Time of the last shot
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private bool facingRight = true; // To track which direction the player is facing
    public Transform firePoint; // Fire point for projectile

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Reference to the Rigidbody2D component
        animator = GetComponent<Animator>(); // Reference to the Animator component
        lastShotTime = 0f; // Initialize last shot time
    }

    void Update()
    {
        // Get horizontal and vertical input (WASD or arrow keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        movement = new Vector2(moveX, moveY) * moveSpeed;

        // Check if the character is moving
        bool isMoving = moveX != 0 || moveY != 0;

        // Set the bool parameter 'isRun' in the animator
        animator.SetBool("isRun", isMoving);

        // Flip the character when moving left
        if (moveX < 0 && facingRight)
        {
            FlipCharacter(); // Flip to face left
        }
        else if (moveX > 0 && !facingRight)
        {
            FlipCharacter(); // Flip to face right
        }

        // Check if the player left-clicks and if enough time has passed since the last shot
        if (Input.GetMouseButtonDown(0) && Time.time >= lastShotTime + fireRate)
        {
            PerformAction(); // Call the function to perform an action
        }
    }

    void FixedUpdate()
    {
        // Move the character
        rb.velocity = movement;
    }

    // Function to flip the character by inverting the x scale
    void FlipCharacter()
    {
        facingRight = !facingRight; // Toggle the facing direction

        Vector3 scale = transform.localScale;
        scale.x *= -1; // Invert the x scale to flip the character
        transform.localScale = scale;
    }

    // Function to perform an action on left-click
    void PerformAction()
    {
        // Set the 'isAttacking' bool in the Animator to true
        animator.SetBool("attack", true);

        // Stop the character's movement
        moveSpeed = 0f;

        // Instantiate the projectile
        ShootProjectile();

        // Update the last shot time
        lastShotTime = Time.time;

        // Optionally, you can add a delay to reset the 'isAttacking' bool to false after the attack animation completes
        StartCoroutine(ResetAttackAnimation());
    }

    // Function to instantiate and shoot the projectile
   void ShootProjectile()
{
   float projectileRotation = facingRight ? 90f : -90f;
    GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, projectileRotation));

    // Get the Rigidbody2D component of the projectile
    Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

    // Set the projectile's velocity based on the player's facing direction
    Vector2 shootDirection = facingRight ? Vector2.right : Vector2.left;
    projectileRb.velocity = shootDirection * projectileSpeed;

    // Assign the hit effect prefab to the projectile
    Projectile projScript = projectile.GetComponent<Projectile>();
    projScript.hitEffectPrefab = hitEffectPrefab; // Use the public variable

    Destroy(projectile, 3f);
}

    // Coroutine to reset the attack animation bool after a short delay
    IEnumerator ResetAttackAnimation()
    {
        // Assuming the attack animation lasts 0.5 seconds, wait before resetting
        yield return new WaitForSeconds(0.5f);
        
        // Set 'isAttacking' back to false
        animator.SetBool("attack", false);
        moveSpeed = 5f;
    }
}

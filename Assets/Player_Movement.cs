using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the player
    public float dashSpeed = 15f; // Dash speed of the player
    public float dashDuration = 0.2f; // Duration of the dash
    public float dashCooldown = 1f; // Cooldown period before the next dash

    public float maxStamina = 100f; // Maximum stamina the player can have
    public float staminaCost = 20f; // Stamina cost for each dash
    public float staminaRegenRate = 10f; // How fast stamina regenerates per second
    public Slider staminaBar; // UI slider to display stamina as a bar
    public TMP_Text staminaText; // TextMeshPro text to display stamina in numbers

    private float currentStamina; // Current stamina the player has
    private Rigidbody2D rb; // Rigidbody2D for movement
    private Vector2 movement; // The direction in which the player is moving
    private Animator animator; // Reference to the Animator component
    private bool facingRight = true; // Tracks which direction the player is facing
    private bool isDashing = false; // Tracks whether the player is currently dashing
    private bool isFiringProjectile = false; // Tracks if the player is firing a projectile
    private float dashCooldownTime; // Time when the next dash can be used

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component for movement
        animator = GetComponent<Animator>(); // Get the Animator component to control animations
        currentStamina = maxStamina; // Initialize stamina to its maximum value

        // Update the UI at the start with full stamina
        if (staminaBar != null) staminaBar.value = 1f;
        if (staminaText != null) staminaText.text = $"{Mathf.Ceil(currentStamina)} / {maxStamina}";
    }

    void Update()
    {
        // Get movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Update movement vector and animator if not currently dashing or firing projectile
        if (!isDashing && !isFiringProjectile)
        {
            movement = new Vector2(moveX, moveY) * moveSpeed;

            // Check if the player is moving and update animation
            bool isMoving = movement != Vector2.zero;
            animator.SetBool("isRun", isMoving);

            // Flip character sprite when changing direction
            if (moveX > 0 && !facingRight) FlipCharacter();
            if (moveX < 0 && facingRight) FlipCharacter();
        }

        // Handle dash input (LeftShift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= dashCooldownTime && currentStamina >= staminaCost)
        {
            StartDash();
        }

        // Regenerate stamina only if not dashing or firing projectile
        if (!isDashing && !isFiringProjectile && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Clamp to max
        }

        // Update stamina UI
        if (staminaBar != null) staminaBar.value = currentStamina / maxStamina; // Update stamina bar
        if (staminaText != null) staminaText.text = $"{Mathf.Ceil(currentStamina)} / {maxStamina}"; // Update the text
    }

    void FixedUpdate()
    {
        // Apply movement if not dashing or firing projectile
        if (!isDashing && !isFiringProjectile)
        {
            rb.velocity = movement;
        }
    }

    void StartDash()
    {
        isDashing = true; // Enable dashing
        dashCooldownTime = Time.time + dashCooldown; // Set cooldown for the next dash
        currentStamina -= staminaCost; // Deduct stamina

        // Trigger dash animation
        animator.SetTrigger("Dash");

        // Apply dash velocity
        rb.velocity = movement.normalized * dashSpeed;

        // Stop dash after the duration
        Invoke(nameof(StopDash), dashDuration);
    }

    void StopDash()
    {
        isDashing = false; // End dashing state
        rb.velocity = Vector2.zero; // Stop the player after dash
    }

    void FlipCharacter()
    {
        facingRight = !facingRight; // Toggle facing direction
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip the character by changing its x scale
        transform.localScale = scale;
    }

    // Public method to stop movement temporarily (used by Player_Attack or other systems)
    public void StopMovement()
    {
        moveSpeed = 0f; // Set movement speed to 0 to stop movement
        rb.velocity = Vector2.zero; // Halt the Rigidbody's movement immediately
    }

    // Public method to resume movement (used by Player_Attack or other systems)
    public void ResumeMovement()
    {
        moveSpeed = 5f; // Reset to default movement speed
    }

    // Call this method when the player is firing a projectile
    public void StartFiringProjectile()
    {
        isFiringProjectile = true; // Set firing state
        currentStamina -= staminaCost; // Consume stamina while firing
    }

    // Call this method when the projectile firing is finished
    public void StopFiringProjectile()
    {
        isFiringProjectile = false; // Reset firing state
    }
}

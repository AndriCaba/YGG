using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import the TextMeshPro namespace

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar; // Reference to the health bar slider
    public TMP_Text healthText; // TextMeshPro for health display
    public GameObject DeathUI;
    private Animator animator;
    private Rigidbody2D rb;
    private Renderer playerRenderer;

    public Color flashColor = Color.red; // Color for damage flash
    public float flashDuration = 0.1f;
    public Player_Movement player_Movement;

    void Start()
    {
        currentHealth = maxHealth; // Set current health to maximum
        UpdateHealthUI();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<Renderer>();
        DeathUI.SetActive(false);
         // Initialize health UI
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage; // Reduce health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        // Flash red when damaged
        StartCoroutine(FlashRed());

        // Knockback effect
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Reset velocity
            rb.AddForce(knockbackDirection.normalized * 10f, ForceMode2D.Impulse);
        }

        if (animator != null)
        {
            animator.SetTrigger("hurt");
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if (playerRenderer != null)
        {
            Color originalColor = playerRenderer.material.color;
            playerRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            playerRenderer.material.color = originalColor;
        }
    }

    private void Die()
    {
        player_Movement.enabled = false;
        Debug.Log("Player has died.");
        if (animator != null)
        {
            animator.SetTrigger("die");
        }
        DeathUI.SetActive(true);
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth; // Normalize health value for slider
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}"; // Update TMP health text
        }
    }
}

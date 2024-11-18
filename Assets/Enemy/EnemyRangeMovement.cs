using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RangeAttack))]
public class EnemyRangeMovement : MonoBehaviour
{
    public float moveSpeed = 3f; // Speed of chasing the player
    public Transform player; // Reference to the player’s transform
    private Animator animator;
    private SpriteRenderer spriteRenderer; // To flip the sprite based on movement direction
    private EnemyAttack enemyAttack; // Reference to the EnemyAttack script

    void Start()
    {
        animator = GetComponent<Animator>(); // Get Animator component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get SpriteRenderer for flipping
        enemyAttack = GetComponent<EnemyAttack>(); // Get the EnemyAttack component
    }

    void Update()
    {
        if (player != null)
        {
            // Only chase the player if out of attack range
            if (Vector2.Distance(transform.position, player.position) > enemyAttack.attackRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                animator.SetBool("isMoving", false); // Stop moving when in attack range
            }
        }
    }

    // Handles movement and animation
    private void MoveTowardsPlayer()
    {
        // Calculate direction towards the player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Trigger movement animation
        if (direction.magnitude > 0f)
        {
            animator.SetBool("isMoving", true); // Set movement animation
            FlipSprite(direction); // Flip sprite based on direction
        }
        else
        {
            animator.SetBool("isMoving", false); // Set idle animation
        }
    }

    // Flips the sprite depending on which direction the enemy is moving
    private void FlipSprite(Vector2 direction)
    {
        if (direction.x < 0f && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true; // Flip sprite to face left
        }
        else if (direction.x > 0f && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false; // Flip sprite to face right
        }
    }

    // Trigger when the enemy enters the player's collider range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform; // Assign player reference
        }
    }

    // Trigger when the enemy exits the player's collider range
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null; // Clear player reference
        }
    }
}

using System.Collections;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    public Transform attackPoint; // Point where the melee attack is centered
    public float attackRange = 0.5f; // Range of the melee attack
    public LayerMask enemyLayers; // Define which layers are considered enemies
    public float comboResetTime = 1f; // Time to reset the combo if no further input
    public float hitDelay = 1f; // Public variable for delay before applying damage

    private int comboStep = 0; // Current step in the combo (0: no attack, 1: first attack, etc.)
    private float lastAttackTime = 0f; // Time of the last attack
    private Animator animator;
    private Player_Movement movementScript; // Reference to Player_Movement script

    void Start()
    {
        animator = GetComponent<Animator>(); // Reference to the Animator component
        movementScript = GetComponent<Player_Movement>(); // Reference to the movement script
    }

    void Update()
    {
        // Check for attack input (left mouse click)
        if (Input.GetMouseButtonDown(0))
        {
            PerformComboAttack(); // Perform combo attack
        }

        // Reset combo if no input after a certain time
        if (Time.time > lastAttackTime + comboResetTime)
        {
            ResetCombo();
        }
    }

    // Perform the combo attack logic
    void PerformComboAttack()
    {
        // Prevent spamming attacks
        if (Time.time < lastAttackTime + 0.2f)
            return;

        // Increment the combo step
        comboStep = Mathf.Clamp(comboStep + 1, 1, 3);

        // Trigger the appropriate animation
        animator.SetTrigger("attack" + comboStep);

        // Stop the character's movement during attack
        movementScript.StopMovement();

        // Perform the attack logic (hit detection)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Start coroutine to apply damage after delay
        StartCoroutine(ApplyDamageWithDelay(hitEnemies));

        // Update the last attack time
        lastAttackTime = Time.time;

        // If the combo is at its last step, reset after a delay
        if (comboStep == 3)
        {
            StartCoroutine(ResetComboWithDelay(0.5f));
        }
    }

    // Coroutine to apply damage after a delay
    IEnumerator ApplyDamageWithDelay(Collider2D[] hitEnemies)
    {
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit with combo step " + comboStep + ": " + enemy.name);

            // Wait for the specified delay before applying damage
            yield return new WaitForSeconds(hitDelay); // Use the public hitDelay variable

            // Check if the enemy has the "Boss" tag before applying damage
            if (enemy.CompareTag("Boss"))
            {
                // Apply damage to the Boss (if the tag is "Boss")
                enemy.GetComponent<Boss>()?.TakeDamage(10, attackPoint.position);
                Debug.Log("Damaged Boss: " + enemy.name);
            }
            else
            {
                // Apply damage to other enemies (if not the "Boss")
                enemy.GetComponent<EnemyHealth>()?.TakeDamage(10, attackPoint.position);
            }
        }
    }

    // Coroutine to reset combo after the final attack
    IEnumerator ResetComboWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetCombo();
        movementScript.ResumeMovement();
    }

    // Reset combo step and resume movement
    void ResetCombo()
    {
        comboStep = 0;
        animator.ResetTrigger("attack1");
        animator.ResetTrigger("attack2");
        animator.ResetTrigger("attack3");
        movementScript.ResumeMovement();
    }

    // Draw the attack range in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private Vector2 movement;
    private Vector2 lastMovement;

    void Awake()
    {
        // Auto-assign components if not set in inspector
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from keyboard
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize diagonal movement so it's not faster
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Track last movement direction for idle animations
        if (movement != Vector2.zero)
        {
            lastMovement = movement;
        }

        // Update animator if available
        UpdateAnimator();

        // Handle interaction input
        if (Input.GetButtonDown("Fire1")) // Default: Left Ctrl or Mouse 0
        {
            Interact();
        }
    }

    void FixedUpdate()
    {
        // Move the player using physics
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        // Set movement parameters for animations
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.magnitude);

        // Set last movement direction for idle animations
        animator.SetFloat("LastHorizontal", lastMovement.x);
        animator.SetFloat("LastVertical", lastMovement.y);
    }

    void Interact()
    {
        // Raycast in the direction the player is facing
        Vector2 interactDirection = lastMovement.normalized;
        float interactDistance = 1f;

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            interactDirection,
            interactDistance
        );

        if (hit.collider != null)
        {
            // Try to get an interactable component
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }

        // Trigger animation if available
        if (animator != null)
        {
            animator.SetTrigger("Interact");
        }

        Debug.Log("Interact button pressed!");
    }

    // Optional: Draw interaction range in editor
    void OnDrawGizmosSelected()
    {
        if (rb == null) return;

        Gizmos.color = Color.yellow;
        Vector2 direction = lastMovement.normalized;
        if (direction == Vector2.zero) direction = Vector2.down;

        Gizmos.DrawLine(rb.position, rb.position + direction * 1f);
    }
}
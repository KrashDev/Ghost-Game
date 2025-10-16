using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float fallDuration = 0.5f;

    [Header("Interaction Settings")]
    public float interactDistance = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    private bool isFalling = false;
    private Vector2 movement;
    private Vector2 lastMovement = Vector2.down; // Default facing down
    private bool facingRight = true;
    [SerializeField] private Animator animator;

    void Start()
    {
        InitializeComponents();
        LoadCharacterSprite();
    }

    void InitializeComponents()
    {
        // Get or add Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody2D component is missing!");
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
        }

        // Get SpriteRenderer - check both on this object and children
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("PlayerController: No SpriteRenderer found on player or children!");
            }
        }

        // Get Collider2D
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("PlayerController: Collider2D component is missing!");
            playerCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Get Animator if available
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Set respawn point to current position if not set
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
            respawnPoint.position = transform.position;
            Debug.Log("PlayerController: Created default respawn point at current position");
        }
    }

    void LoadCharacterSprite()
    {
        if (GameManager.Instance != null && GameManager.Instance.characters.Length > 0)
        {
            CharacterData selectedCharacter = GameManager.Instance.characters[GameManager.Instance.selectedCharacterIndex];

            if (spriteRenderer != null && selectedCharacter.characterSprite != null)
            {
                spriteRenderer.sprite = selectedCharacter.characterSprite;
                Debug.Log($"Loaded character sprite: {selectedCharacter.characterName}");
            }
        }
    }

    void Update()
    {
        if (!isFalling)
        {
            // Get input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Normalize diagonal movement
            if (movement.magnitude > 1)
            {
                movement.Normalize();
            }

            // Track last movement direction for interactions and idle animations
            if (movement != Vector2.zero)
            {
                lastMovement = movement;
            }

            // Handle sprite flipping based on movement direction
            if (movement.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (movement.x < 0 && facingRight)
            {
                Flip();
            }

            // Update animator
            UpdateAnimator();

            // Handle interaction input - SPACEBAR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Interact();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isFalling && rb != null)
        {
            // Move player
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void Flip()
    {
        // Toggle the facing direction
        facingRight = !facingRight;

        // Flip the player sprite by inverting the x scale
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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
                Debug.Log($"Interacted with: {hit.collider.name}");
            }
        }

        // Trigger animation if available
        if (animator != null)
        {
            animator.SetTrigger("Interact");
        }

        Debug.Log("Interact button pressed!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hole") && !isFalling)
        {
            // CHECK FOR HULA LEI BEFORE FALLING!
            if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanWalkOverHoles())
            {
                Debug.Log("Walking over hole safely with Hula Lei!");
                return; // Don't fall if player has Hula Lei
            }

            // Only fall if player doesn't have Hula Lei
            Debug.Log("Falling into hole - no Hula Lei!");
            StartFalling();
        }
    }

    void StartFalling()
    {
        // Safety check before starting fall
        if (spriteRenderer == null)
        {
            Debug.LogError("PlayerController: Cannot fall - SpriteRenderer is null!");
            Respawn(); // Just respawn without animation
            return;
        }

        isFalling = true;
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // Start fall animation
        StartCoroutine(FallAndRespawn());
    }

    IEnumerator FallAndRespawn()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        // Shrink and fade out
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fallDuration;

            // Scale down
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);

            // Fade out (with null check)
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f - progress;
                spriteRenderer.color = color;
            }

            yield return null;
        }

        // Respawn
        Respawn();
    }

    void Respawn()
    {
        // Reset position
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            Debug.LogWarning("PlayerController: No respawn point set!");
        }

        // Reset scale and alpha
        transform.localScale = Vector3.one;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        // Re-enable
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        isFalling = false;

        // Reset movement
        movement = Vector2.zero;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    // Method to set respawn point dynamically
    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
        }
        respawnPoint.position = newRespawnPoint;
    }

    // Optional: Draw interaction range in editor
    void OnDrawGizmosSelected()
    {
        if (rb == null) return;

        Gizmos.color = Color.yellow;
        Vector2 direction = lastMovement.normalized;
        if (direction == Vector2.zero) direction = Vector2.down;

        Vector2 position = Application.isPlaying ? rb.position : (Vector2)transform.position;
        Gizmos.DrawLine(position, position + direction * interactDistance);
    }

    // Debug method to check component setup
    void OnValidate()
    {
        if (Application.isPlaying) return;

        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogWarning("PlayerController: Missing Rigidbody2D component!");
        }
        if (GetComponent<SpriteRenderer>() == null && GetComponentInChildren<SpriteRenderer>() == null)
        {
            Debug.LogWarning("PlayerController: Missing SpriteRenderer component!");
        }
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("PlayerController: Missing Collider2D component!");
        }
    }
}
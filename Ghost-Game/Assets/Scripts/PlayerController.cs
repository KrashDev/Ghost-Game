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

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    private bool isFalling = false;
    private Vector2 movement;

    void Start()
    {
        InitializeComponents();
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

        // Set respawn point to current position if not set
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
            respawnPoint.position = transform.position;
            Debug.Log("PlayerController: Created default respawn point at current position");
        }
    }

    void Update()
    {
        if (!isFalling)
        {
            // Get input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
    }

    void FixedUpdate()
    {
        if (!isFalling && rb != null)
        {
            // Move player
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
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

    // Optional: Method to set respawn point dynamically
    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
        }
        respawnPoint.position = newRespawnPoint;
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
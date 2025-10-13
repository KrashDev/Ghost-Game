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
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();

        // Set respawn point to current position if not set
        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
            respawnPoint.position = transform.position;
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
        if (!isFalling)
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
        isFalling = true;
        playerCollider.enabled = false;

        // Start fall animation
        StartCoroutine(FallAndRespawn());
    }

    System.Collections.IEnumerator FallAndRespawn()
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

            // Fade out
            Color color = spriteRenderer.color;
            color.a = 1f - progress;
            spriteRenderer.color = color;

            yield return null;
        }

        // Respawn
        Respawn();
    }

    void Respawn()
    {
        // Reset position
        transform.position = respawnPoint.position;

        // Reset scale and alpha
        transform.localScale = Vector3.one;
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;

        // Re-enable
        playerCollider.enabled = true;
        isFalling = false;

        // Reset movement
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
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
}
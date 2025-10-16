using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SimpleHole : MonoBehaviour
{
    [Header("Hole Settings")]
    [SerializeField] private bool requiresHulaLei = true;
    [SerializeField] private float fallDelay = 0.3f;
    [SerializeField] private Transform holeRespawnPoint; // REQUIRED: Each hole should have its own respawn point

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color passableColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Effects")]
    [SerializeField] private AudioClip fallSound;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isProcessingFall = false;

    private void Awake()
    {
        // Ensure trigger collider
        GetComponent<Collider2D>().isTrigger = true;

        // Setup components
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && fallSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Validation
        if (holeRespawnPoint == null)
        {
            Debug.LogError($"Hole '{gameObject.name}' has no respawn point assigned! Player will respawn at (0,0,0)");
        }
    }

    private void Update()
    {
        // Update visual to show if hole is passable
        if (spriteRenderer != null && requiresHulaLei)
        {
            bool canWalkOver = PlayerInventory.Instance != null &&
                             PlayerInventory.Instance.CanWalkOverHoles();
            spriteRenderer.color = canWalkOver ? passableColor : normalColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isProcessingFall) return;

        if (other.CompareTag("Player"))
        {
            CheckPlayerFall(other.gameObject);
        }
    }

    private void CheckPlayerFall(GameObject player)
    {
        // Check if player can walk over holes
        if (requiresHulaLei && PlayerInventory.Instance != null)
        {
            if (PlayerInventory.Instance.CanWalkOverHoles())
            {
                // Player has Hula Lei - they can pass
                return;
            }
        }

        // Player falls!
        StartCoroutine(ProcessPlayerFall(player));
    }

    private IEnumerator ProcessPlayerFall(GameObject player)
    {
        isProcessingFall = true;

        // Get player controller
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Player has no PlayerController!");
            isProcessingFall = false;
            yield break;
        }

        // Disable player control
        playerController.enabled = false;

        // Play fall sound
        if (fallSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fallSound);
        }

        // Visual feedback - fade out
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            float fadeTime = fallDelay;
            float elapsed = 0f;
            Color originalColor = playerSprite.color;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                playerSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }

        // Teleport to THIS HOLE's specific respawn point
        if (holeRespawnPoint != null)
        {
            player.transform.position = holeRespawnPoint.position;
        }
        else
        {
            Debug.LogError($"Hole '{gameObject.name}' has no respawn point! Cannot teleport player.");
        }

        // Restore player visual
        if (playerSprite != null)
        {
            playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1f);
        }

        // Re-enable player control
        playerController.enabled = true;

        // Brief cooldown
        yield return new WaitForSeconds(0.5f);
        isProcessingFall = false;
    }

    private void OnDrawGizmos()
    {
        // Draw the hole
        Gizmos.color = new Color(0, 0, 0, 0.5f);
        Collider2D col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, new Vector3(box.size.x, box.size.y, 0.1f));
        }
        else if (col is CircleCollider2D circle)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(circle.offset, circle.radius);
        }

        // Draw line to respawn point if set
        if (holeRespawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, holeRespawnPoint.position);
            Gizmos.DrawWireSphere(holeRespawnPoint.position, 0.3f);
        }
        else
        {
            // Warning indicator if no respawn point
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
    }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTeleporter : MonoBehaviour
{
    [Header("Teleport Destination")]
    [Tooltip("The teleporter where the player will appear")]
    [SerializeField] private SimpleTeleporter destinationTeleporter;

    [Header("Settings")]
    [SerializeField] private float teleportDelay = 0.2f;
    [SerializeField] private float cooldownTime = 0.5f;

    [Header("Visual Feedback (Optional)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color teleportColor = Color.white;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip teleportSound;

    [Header("Effects (Optional)")]
    [SerializeField] private ParticleSystem teleportEffect;

    private AudioSource audioSource;
    private bool isOnCooldown = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;

        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        if (destinationTeleporter == null)
        {
            Debug.LogWarning($"Teleporter '{gameObject.name}' has no destination set!");
            return;
        }

        StartCoroutine(TeleportSequence(player));
    }

    private System.Collections.IEnumerator TeleportSequence(GameObject player)
    {
        isOnCooldown = true;

        // Visual feedback
        if (spriteRenderer != null)
        {
            spriteRenderer.color = teleportColor;
        }

        // Play sound
        if (audioSource != null && teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // Play effect
        if (teleportEffect != null)
        {
            teleportEffect.Play();
        }

        // Optional: Disable player control during teleport
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        yield return new WaitForSeconds(teleportDelay);

        // Teleport to destination
        player.transform.position = destinationTeleporter.transform.position;

        // Play effect at destination
        if (destinationTeleporter.teleportEffect != null)
        {
            destinationTeleporter.teleportEffect.Play();
        }

        // Re-enable player control
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Reset visual
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }

        // Put destination on cooldown too
        destinationTeleporter.StartCooldown();

        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    public void StartCooldown()
    {
        StartCoroutine(CooldownTimer());
    }

    private System.Collections.IEnumerator CooldownTimer()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    // Visualize connection in editor
    private void OnDrawGizmos()
    {
        if (destinationTeleporter != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationTeleporter.transform.position);

            // Draw arrow at destination
            Vector3 direction = (destinationTeleporter.transform.position - transform.position).normalized;
            Vector3 arrowPos = destinationTeleporter.transform.position;
            Gizmos.DrawSphere(arrowPos, 0.2f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw range indicator
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
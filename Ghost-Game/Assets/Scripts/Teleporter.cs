using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    [SerializeField] private int teleporterID;
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private bool isEntryPoint = false;

    [Header("Sequence Settings")]
    [SerializeField] private int requiredSequenceNumber = -1; // -1 means no sequence required
    [SerializeField] private bool resetSequenceOnWrongEntry = true;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioClip teleportSound;
    [SerializeField] private AudioClip correctSequenceSound;
    [SerializeField] private AudioClip wrongSequenceSound;

    [Header("Effects")]
    [SerializeField] private ParticleSystem teleportEffect;
    [SerializeField] private float teleportDelay = 0.2f;

    private TeleporterManager manager;
    private AudioSource audioSource;
    private bool isTeleporting = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        manager = TeleporterManager.Instance;
        if (manager != null)
        {
            manager.RegisterTeleporter(this);
        }

        UpdateVisualState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;

        if (other.CompareTag("Player"))
        {
            AttemptTeleport(other.gameObject);
        }
    }

    private void AttemptTeleport(GameObject player)
    {
        if (destinationPoint == null)
        {
            Debug.LogWarning($"Teleporter {teleporterID} has no destination point!");
            return;
        }

        // Check if this teleporter requires a sequence
        if (requiredSequenceNumber >= 0)
        {
            int currentSequence = manager.GetCurrentSequence();

            if (currentSequence == requiredSequenceNumber)
            {
                // Correct sequence!
                manager.AdvanceSequence();
                StartCoroutine(TeleportPlayer(player, true));
            }
            else
            {
                // Wrong sequence!
                if (resetSequenceOnWrongEntry)
                {
                    manager.ResetSequence();
                }
                StartCoroutine(ShowWrongSequenceFeedback());
            }
        }
        else
        {
            // No sequence required, just teleport
            StartCoroutine(TeleportPlayer(player, false));
        }
    }

    private System.Collections.IEnumerator TeleportPlayer(GameObject player, bool correctSequence)
    {
        isTeleporting = true;

        // Play audio
        if (correctSequence && correctSequenceSound != null)
        {
            audioSource.PlayOneShot(correctSequenceSound);
        }
        else if (teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // Visual feedback
        if (spriteRenderer != null)
        {
            spriteRenderer.color = correctSequence ? correctColor : activeColor;
        }

        // Play particle effect
        if (teleportEffect != null)
        {
            teleportEffect.Play();
        }

        // Disable player control during teleport
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        yield return new WaitForSeconds(teleportDelay);

        // Teleport the player
        player.transform.position = destinationPoint.position;

        // Play effect at destination
        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, destinationPoint.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.1f);

        // Re-enable player control
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        UpdateVisualState();

        yield return new WaitForSeconds(0.5f);
        isTeleporting = false;
    }

    private System.Collections.IEnumerator ShowWrongSequenceFeedback()
    {
        // Play wrong sound
        if (wrongSequenceSound != null)
        {
            audioSource.PlayOneShot(wrongSequenceSound);
        }

        // Flash red
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;

            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = wrongColor;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }

            UpdateVisualState();
        }
    }

    public void UpdateVisualState()
    {
        if (spriteRenderer == null) return;

        if (requiredSequenceNumber < 0)
        {
            // Always active if no sequence required
            spriteRenderer.color = activeColor;
        }
        else
        {
            int currentSequence = manager != null ? manager.GetCurrentSequence() : 0;

            if (currentSequence == requiredSequenceNumber)
            {
                spriteRenderer.color = activeColor;
            }
            else
            {
                spriteRenderer.color = inactiveColor;
            }
        }
    }

    public int GetTeleporterID() => teleporterID;
    public int GetRequiredSequenceNumber() => requiredSequenceNumber;
    public bool IsEntryPoint() => isEntryPoint;

    private void OnDrawGizmos()
    {
        if (destinationPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationPoint.position);
            Gizmos.DrawWireSphere(destinationPoint.position, 0.3f);
        }

        // Draw sequence number
#if UNITY_EDITOR
        if (requiredSequenceNumber >= 0)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
                $"Seq: {requiredSequenceNumber}");
        }
#endif
    }
}
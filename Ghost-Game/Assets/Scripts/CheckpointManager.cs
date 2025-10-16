using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager instance;
    public static CheckpointManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CheckpointManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("CheckpointManager");
                    instance = go.AddComponent<CheckpointManager>();
                }
            }
            return instance;
        }
    }

    [Header("Checkpoint Settings")]
    [SerializeField] private Transform defaultSpawnPoint;
    [SerializeField] private Transform currentCheckpoint;

    [Header("Visual Feedback")]
    [SerializeField] private bool showCheckpointActivation = true;
    [SerializeField] private float checkpointFlashDuration = 0.5f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Find the player and set initial spawn point
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && defaultSpawnPoint == null)
        {
            defaultSpawnPoint = player.transform;
        }

        currentCheckpoint = defaultSpawnPoint;
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log($"Checkpoint set at: {newCheckpoint.position}");

        // Update player's respawn point
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.respawnPoint = newCheckpoint;
        }
    }

    public Transform GetCurrentCheckpoint()
    {
        return currentCheckpoint != null ? currentCheckpoint : defaultSpawnPoint;
    }

    public Vector3 GetRespawnPosition()
    {
        Transform checkpoint = GetCurrentCheckpoint();
        return checkpoint != null ? checkpoint.position : Vector3.zero;
    }
}

// Simple checkpoint trigger
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool autoActivate = true;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color activeColor = Color.green;

    [Header("Audio")]
    [SerializeField] private AudioClip activationSound;

    private AudioSource audioSource;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && activationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        UpdateVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!autoActivate || isActivated) return;

        if (other.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        if (isActivated) return;

        isActivated = true;
        CheckpointManager.Instance.SetCheckpoint(transform);

        // Visual feedback
        UpdateVisual();

        // Audio feedback
        if (activationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(activationSound);
        }

        // Optional: Add particle effect or animation
    }

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isActivated ? activeColor : inactiveColor;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isActivated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.7f, 
            isActivated ? "Checkpoint (Active)" : "Checkpoint");
#endif
    }
}
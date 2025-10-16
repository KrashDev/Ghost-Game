using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("Visual Settings")]
    public bool showGizmo = true;
    public Color gizmoColor = Color.green;
    public float gizmoRadius = 0.5f;

    [Header("Checkpoint Settings")]
    [Tooltip("If true, this acts as a checkpoint - player must touch it to activate")]
    public bool isCheckpoint = false;

    [Tooltip("If true, assigns as respawn point when scene starts (for initial spawn only)")]
    public bool isInitialSpawnPoint = false;

    private bool hasBeenActivated = false;

    void Start()
    {
        // Only auto-assign if this is marked as the initial spawn point
        if (isInitialSpawnPoint)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.respawnPoint = this.transform;
                Debug.Log($"Initial spawn point at {transform.position} assigned to player.");
            }
            else
            {
                Debug.LogWarning("No PlayerController found in scene to assign initial spawn point.");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only checkpoints should update the player's respawn point when touched
        if (isCheckpoint && !hasBeenActivated && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.respawnPoint = this.transform;
                hasBeenActivated = true;
                Debug.Log($"Checkpoint activated at {transform.position}");

                // Optional: Add visual/audio feedback here
                OnCheckpointActivated();
            }
        }
    }

    private void OnCheckpointActivated()
    {
        // Override this or add effects here
        // Examples:
        // - Play checkpoint sound
        // - Show particle effect
        // - Change color
        // - Save game
    }

    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            // Different colors for different types
            Color drawColor = gizmoColor;
            if (isInitialSpawnPoint)
            {
                drawColor = Color.cyan; // Cyan for initial spawn
            }
            else if (isCheckpoint)
            {
                drawColor = Color.yellow; // Yellow for checkpoints
            }
            else
            {
                drawColor = Color.green; // Green for hole respawn points
            }

            Gizmos.color = drawColor;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);

            // Draw arrow pointing up
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * gizmoRadius);

            // Draw X and Z axes
            Gizmos.color = new Color(drawColor.r, drawColor.g, drawColor.b, 0.5f);
            Gizmos.DrawLine(transform.position - Vector3.right * gizmoRadius,
                           transform.position + Vector3.right * gizmoRadius);
            Gizmos.DrawLine(transform.position - Vector3.forward * gizmoRadius,
                           transform.position + Vector3.forward * gizmoRadius);

#if UNITY_EDITOR
            // Label
            string label = "";
            if (isInitialSpawnPoint) label = "Initial Spawn";
            else if (isCheckpoint) label = "Checkpoint";
            else label = "Respawn Point";
            
            UnityEditor.Handles.Label(transform.position + Vector3.up * (gizmoRadius + 0.3f), label);
#endif
        }
    }
}
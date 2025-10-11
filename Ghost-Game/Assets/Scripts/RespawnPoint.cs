using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("Visual Settings")]
    public bool showGizmo = true;
    public Color gizmoColor = Color.green;
    public float gizmoRadius = 0.5f;

    [Header("Auto-Assign to Player")]
    public bool autoAssignToPlayer = true;

    void Start()
    {
        if (autoAssignToPlayer)
        {
            // Find player and assign this as respawn point
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.respawnPoint = this.transform;
                Debug.Log($"Respawn point at {transform.position} assigned to player.");
            }
            else
            {
                Debug.LogWarning("No PlayerController found in scene to assign respawn point.");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);

            // Draw arrow pointing up
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * gizmoRadius);

            // Draw X and Z axes
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);
            Gizmos.DrawLine(transform.position - Vector3.right * gizmoRadius,
                           transform.position + Vector3.right * gizmoRadius);
            Gizmos.DrawLine(transform.position - Vector3.forward * gizmoRadius,
                           transform.position + Vector3.forward * gizmoRadius);
        }
    }
}
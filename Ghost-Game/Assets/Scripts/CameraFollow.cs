using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player transform to follow")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("How smoothly the camera follows (lower = smoother)")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    [Tooltip("Offset from the player position")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Optional: Boundary Limits")]
    [Tooltip("Enable to constrain camera within boundaries")]
    public bool useBoundaries = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target not set!");
            return;
        }

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between current and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply boundary constraints if enabled
        if (useBoundaries)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        // Update camera position
        transform.position = smoothedPosition;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    [Header("Hole Settings")]
    public bool showDebugGizmo = true;
    public Color gizmoColor = new Color(0, 0, 0, 0.5f);

    void Start()
    {
        // Ensure this GameObject has the "Hole" tag
        if (!gameObject.CompareTag("Hole"))
        {
            Debug.LogWarning($"HoleTrigger on {gameObject.name} does not have 'Hole' tag. Adding it now.");
            gameObject.tag = "Hole";
        }

        // Ensure there's a trigger collider
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError($"HoleTrigger on {gameObject.name} requires a Collider2D component!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} is not set as trigger. Setting it now.");
            col.isTrigger = true;
        }
    }

    void OnDrawGizmos()
    {
        if (showDebugGizmo)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = gizmoColor;

                if (col is BoxCollider2D)
                {
                    BoxCollider2D box = (BoxCollider2D)col;
                    Vector3 size = new Vector3(box.size.x, box.size.y, 0.1f);
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(box.offset, size);
                }
                else if (col is CircleCollider2D)
                {
                    CircleCollider2D circle = (CircleCollider2D)col;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawSphere(circle.offset, circle.radius);
                }
            }
        }
    }
}
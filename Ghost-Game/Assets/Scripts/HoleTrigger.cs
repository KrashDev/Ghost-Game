using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    [Header("Hole Settings")]
    public bool showDebugGizmo = true;
    public Color gizmoColor = new Color(0, 0, 0, 0.5f);
    public bool requiresHulaLei = true;

    private Collider2D holeCollider;

    void Start()
    {
        // Ensure this GameObject has the "Hole" tag
        if (!gameObject.CompareTag("Hole"))
        {
            Debug.LogWarning($"HoleTrigger on {gameObject.name} does not have 'Hole' tag. Adding it now.");
            gameObject.tag = "Hole";
        }

        // Ensure there's a trigger collider
        holeCollider = GetComponent<Collider2D>();
        if (holeCollider == null)
        {
            Debug.LogError($"HoleTrigger on {gameObject.name} requires a Collider2D component!");
        }
        else if (!holeCollider.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} is not set as trigger. Setting it now.");
            holeCollider.isTrigger = true;
        }
    }

    void Update()
    {
        // Disable hole collider if player can walk over holes
        if (requiresHulaLei && PlayerInventory.Instance != null && holeCollider != null)
        {
            bool canWalkOverHoles = PlayerInventory.Instance.CanWalkOverHoles();

            // Disable the collider when player has Hula Lei
            holeCollider.enabled = !canWalkOverHoles;

            if (canWalkOverHoles)
            {
                // Optional: Change the tag so other systems don't detect it as a hole
                if (gameObject.tag == "Hole")
                {
                    gameObject.tag = "Untagged";
                }
            }
            else
            {
                // Re-enable hole tag when Hula Lei is not equipped
                if (gameObject.tag != "Hole")
                {
                    gameObject.tag = "Hole";
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (showDebugGizmo)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                // Change color based on whether it's active
                Color drawColor = gizmoColor;
                if (Application.isPlaying && PlayerInventory.Instance != null &&
                    PlayerInventory.Instance.CanWalkOverHoles())
                {
                    drawColor = new Color(0, 1, 0, 0.3f); // Green when passable
                }

                Gizmos.color = drawColor;

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
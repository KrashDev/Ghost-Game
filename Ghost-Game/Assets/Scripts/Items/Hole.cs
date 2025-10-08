using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hole : MonoBehaviour
{
    [Header("Hole Properties")]
    public bool requiresHoverBoots = true;

    private void Start()
    {
        // Make sure the collider is a trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerEnter(other.gameObject);
        }
    }

    private void HandlePlayerEnter(GameObject player)
    {
        if (!requiresHoverBoots) return;

        // Check if player has hover boots equipped
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanWalkOverHoles())
        {
            // Player can walk over the hole
            Debug.Log("Walking over hole with Hover Boots!");
        }
        else
        {
            // Player falls or is blocked
            HandlePlayerFall(player);
        }
    }

    private void HandlePlayerFall(GameObject player)
    {
        // You can implement different behaviors:
        // 1. Respawn player at checkpoint
        // 2. Take damage
        // 3. Block movement

        Debug.Log("Cannot cross hole without Hover Boots!");

        // Example: Push player back
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            // Reset player position slightly
            Vector3 pushDirection = (player.transform.position - transform.position).normalized;
            player.transform.position += pushDirection * 0.5f;
        }
    }
}
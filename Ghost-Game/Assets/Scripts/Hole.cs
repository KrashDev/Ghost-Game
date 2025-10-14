using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hole : MonoBehaviour
{
    [Header("Hole Properties")]
    public bool requiresHulaLei = true;

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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerEnter(other.gameObject);
        }
    }

    private void HandlePlayerEnter(GameObject player)
    {
        if (!requiresHulaLei)
        {
            // Hole doesn't require item, player can pass freely
            return;
        }

        // Check if player has Hula Lei equipped
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanWalkOverHoles())
        {
            // Player can walk over the hole safely - do nothing
            Debug.Log("Walking over hole with Hula Lei!");
            return; // IMPORTANT: Exit here, don't fall!
        }

        // If we reach here, player doesn't have Hula Lei - make them fall
        HandlePlayerFall(player);
    }

    private void HandlePlayerFall(GameObject player)
    {
        // Player falls into the hole
        Debug.Log("Cannot cross hole without Hula Lei!");

        // Push player back to prevent them from crossing
        Vector3 pushDirection = (player.transform.position - transform.position).normalized;
        player.transform.position += pushDirection * 0.5f;

        // Optional: Add additional fall behavior here
        // - Respawn at checkpoint
        // - Take damage
        // - Play fall animation
        // - Play fall sound
    }
}
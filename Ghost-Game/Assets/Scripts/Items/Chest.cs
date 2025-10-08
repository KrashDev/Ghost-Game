using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest Properties")]
    public Item containedItem;
    public bool isOpen = false;

    [Header("Visual")]
    public SpriteRenderer chestRenderer;
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Interaction")]
    public float interactionRange = 1.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Item Display")]
    public GameObject itemDisplayPrefab;
    public Vector3 itemDisplayOffset = new Vector3(0, 1f, 0);

    private bool playerInRange = false;
    private GameObject itemDisplay;

    private void Start()
    {
        if (chestRenderer != null && closedSprite != null)
        {
            chestRenderer.sprite = closedSprite;
        }
    }

    private void Update()
    {
        CheckPlayerProximity();

        if (playerInRange && !isOpen && Input.GetKeyDown(interactKey))
        {
            OpenChest();
        }
    }

    private void CheckPlayerProximity()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        playerInRange = distance <= interactionRange;
    }

    private void OpenChest()
    {
        if (isOpen || containedItem == null) return;

        isOpen = true;

        // Change sprite
        if (chestRenderer != null && openSprite != null)
        {
            chestRenderer.sprite = openSprite;
        }

        // Give item to player
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddItem(containedItem);
        }

        // Show item popup
        ShowItemAcquired();

        // Play sound (if you have audio)
        // AudioManager.Instance.PlaySound("ChestOpen");
    }

    private void ShowItemAcquired()
    {
        if (itemDisplayPrefab != null)
        {
            itemDisplay = Instantiate(itemDisplayPrefab, transform.position + itemDisplayOffset, Quaternion.identity);

            // Set the item icon
            SpriteRenderer sr = itemDisplay.GetComponent<SpriteRenderer>();
            if (sr != null && containedItem.icon != null)
            {
                sr.sprite = containedItem.icon;
            }

            // Destroy after delay
            Destroy(itemDisplay, 2f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
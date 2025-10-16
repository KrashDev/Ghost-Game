using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMesh Pro namespace

public class Chest : MonoBehaviour
{
    [Header("Chest Properties")]
    public Item containedItem;
    public bool isOpen = false;

    [Header("Visual")]
    public SpriteRenderer chestRenderer;
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Audio")]
    public AudioClip openChest;
    private AudioSource audioSource;

    [Header("Interaction")]
    public float interactionRange = 1.5f;
    public KeyCode interactKey = KeyCode.Space;
    public KeyCode closeUIKey = KeyCode.Space;

    [Header("Item Display")]
    public GameObject itemDisplayPrefab;
    public Vector3 itemDisplayOffset = new Vector3(0, 1f, 0);

    [Header("UI Panel")]
    public GameObject itemUIPanel;
    public UnityEngine.UI.Image itemUIIcon;
    public TextMeshProUGUI itemUIName;
    public TextMeshProUGUI itemUIDescription;

    private bool playerInRange = false;
    private GameObject itemDisplay;
    private bool isUIOpen = false;

    private void Start()
    {
        if (chestRenderer != null && closedSprite != null)
        {
            chestRenderer.sprite = closedSprite;
        }

        // Hide UI panel at start
        if (itemUIPanel != null)
        {
            itemUIPanel.SetActive(false);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckPlayerProximity();

        // Handle UI panel closing
        if (isUIOpen && Input.GetKeyDown(closeUIKey))
        {
            CloseUI();
        }

        // Handle chest opening
        if (playerInRange && !isOpen && !isUIOpen && Input.GetKeyDown(interactKey))
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

        // Show item popup in world
        ShowItemAcquired();

        // Show UI panel
        ShowItemUI();

        // Play sound
        audioSource.clip = openChest;
        audioSource.Play();
    }

    private void ShowItemUI()
    {
        if (itemUIPanel == null || containedItem == null) return;

        // Update UI elements
        if (itemUIIcon != null && containedItem.icon != null)
        {
            itemUIIcon.sprite = containedItem.icon;
            itemUIIcon.enabled = true;
        }

        if (itemUIName != null)
        {
            itemUIName.text = containedItem.itemName;
        }

        if (itemUIDescription != null)
        {
            itemUIDescription.text = containedItem.description;
        }

        // Show the panel
        itemUIPanel.SetActive(true);
        isUIOpen = true;

        // Pause game or disable player movement if desired
        // Time.timeScale = 0f; // Uncomment to pause game
    }

    private void CloseUI()
    {
        if (itemUIPanel != null)
        {
            itemUIPanel.SetActive(false);
        }

        isUIOpen = false;

        // Resume game if you paused it
        // Time.timeScale = 1f; // Uncomment if you paused game
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
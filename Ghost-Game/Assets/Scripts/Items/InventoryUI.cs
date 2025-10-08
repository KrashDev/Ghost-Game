using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemIcon;
    public Text itemName;
    public Text itemDescription;
    public GameObject itemPanel;

    [Header("Settings")]
    public float displayDuration = 3f;

    private float displayTimer = 0f;

    private void Start()
    {
        if (itemPanel != null)
        {
            itemPanel.SetActive(false);
        }

        // Subscribe to inventory events
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnItemEquipped += DisplayItem;
        }
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnItemEquipped -= DisplayItem;
        }
    }

    private void Update()
    {
        if (displayTimer > 0)
        {
            displayTimer -= Time.deltaTime;

            if (displayTimer <= 0 && itemPanel != null)
            {
                itemPanel.SetActive(false);
            }
        }
    }

    public void DisplayItem(Item item)
    {
        if (item == null)
        {
            if (itemPanel != null)
            {
                itemPanel.SetActive(false);
            }
            return;
        }

        // Update UI elements
        if (itemIcon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = item.icon != null;
        }

        if (itemName != null)
        {
            itemName.text = item.itemName;
        }

        if (itemDescription != null)
        {
            itemDescription.text = item.description;
        }

        // Show panel
        if (itemPanel != null)
        {
            itemPanel.SetActive(true);
        }

        displayTimer = displayDuration;
    }
}
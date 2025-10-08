using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Equipped Items")]
    public Item equippedItem;

    [Header("Visual")]
    public SpriteRenderer itemOverlayRenderer;

    private HashSet<ItemType> collectedItems = new HashSet<ItemType>();

    // Events
    public delegate void ItemEquipped(Item item);
    public event ItemEquipped OnItemEquipped;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (itemOverlayRenderer != null)
        {
            itemOverlayRenderer.enabled = false;
        }
    }

    public void AddItem(Item item)
    {
        if (item == null) return;

        collectedItems.Add(item.itemType);
        EquipItem(item);
    }

    public void EquipItem(Item item)
    {
        equippedItem = item;

        // Update visual overlay
        if (itemOverlayRenderer != null && item != null)
        {
            if (item.characterOverlay != null)
            {
                itemOverlayRenderer.sprite = item.characterOverlay;
                itemOverlayRenderer.enabled = true;
            }
            else
            {
                itemOverlayRenderer.enabled = false;
            }
        }
        else if (itemOverlayRenderer != null)
        {
            itemOverlayRenderer.enabled = false;
        }

        OnItemEquipped?.Invoke(item);
    }

    public bool HasItem(ItemType type)
    {
        return collectedItems.Contains(type);
    }

    public bool CanWalkOverHoles()
    {
        return equippedItem != null && equippedItem.canWalkOverHoles;
    }

    public bool CanSeeHidden()
    {
        return equippedItem != null && equippedItem.canSeeHidden;
    }

    public bool CanPassThroughWalls()
    {
        return equippedItem != null && equippedItem.canPassThroughWalls;
    }
}
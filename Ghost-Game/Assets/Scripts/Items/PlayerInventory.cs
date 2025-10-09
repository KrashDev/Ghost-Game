using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Equipped Items")]
    public List<Item> equippedItems = new List<Item>();

    [Header("Visual Overlays")]
    public SpriteRenderer hulaLeiOverlay;
    public SpriteRenderer sunShadesOverlay;
    public SpriteRenderer partyHatOverlay;
    public Vector3 itemOffset = new Vector3(0, 0.5f, 0);

    private Dictionary<ItemType, Item> itemCollection = new Dictionary<ItemType, Item>();

    // Events
    public delegate void ItemEquipped(Item item);
    public event ItemEquipped OnItemEquipped;

    public delegate void ItemRemoved(Item item);
    public event ItemRemoved OnItemRemoved;

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
        SetupOverlays();
    }

    private void SetupOverlays()
    {
        // Setup each overlay renderer
        if (hulaLeiOverlay != null)
        {
            hulaLeiOverlay.enabled = false;
            hulaLeiOverlay.sortingOrder = 11;
            hulaLeiOverlay.transform.localPosition = itemOffset;
        }

        if (sunShadesOverlay != null)
        {
            sunShadesOverlay.enabled = false;
            sunShadesOverlay.sortingOrder = 11;
            sunShadesOverlay.transform.localPosition = itemOffset;
        }

        if (partyHatOverlay != null)
        {
            partyHatOverlay.enabled = false;
            partyHatOverlay.sortingOrder = 12;
            partyHatOverlay.transform.localPosition = itemOffset;
        }
    }

    public void AddItem(Item item)
    {
        if (item == null) return;

        // Add to collection if not already present
        if (!itemCollection.ContainsKey(item.itemType))
        {
            itemCollection.Add(item.itemType, item);
            equippedItems.Add(item);

            UpdateItemVisual(item, true);
            OnItemEquipped?.Invoke(item);

            Debug.Log($"Acquired {item.itemName}!");
        }
    }

    public void RemoveItem(ItemType itemType)
    {
        if (itemCollection.ContainsKey(itemType))
        {
            Item item = itemCollection[itemType];
            itemCollection.Remove(itemType);
            equippedItems.Remove(item);

            UpdateItemVisual(item, false);
            OnItemRemoved?.Invoke(item);
        }
    }

    private void UpdateItemVisual(Item item, bool show)
    {
        SpriteRenderer targetOverlay = GetOverlayForItem(item.itemType);

        if (targetOverlay != null)
        {
            if (show && item.characterOverlay != null)
            {
                targetOverlay.sprite = item.characterOverlay;
                targetOverlay.enabled = true;
            }
            else
            {
                targetOverlay.enabled = false;
            }
        }
    }

    private SpriteRenderer GetOverlayForItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.HulaLei:
                return hulaLeiOverlay;
            case ItemType.SunShades:
                return sunShadesOverlay;
            case ItemType.PartyHat:
                return partyHatOverlay;
            default:
                return null;
        }
    }

    public bool HasItem(ItemType type)
    {
        return itemCollection.ContainsKey(type);
    }

    public bool CanWalkOverHoles()
    {
        return HasItem(ItemType.HulaLei);
    }

    public bool CanSeeHidden()
    {
        return HasItem(ItemType.SunShades);
    }

    public bool CanPassThroughWalls()
    {
        return HasItem(ItemType.PartyHat);
    }

    // Get all active abilities as a readable string
    public string GetActiveAbilities()
    {
        List<string> abilities = new List<string>();

        if (CanWalkOverHoles()) abilities.Add("Walk Over Holes");
        if (CanSeeHidden()) abilities.Add("See Hidden");
        if (CanPassThroughWalls()) abilities.Add("Pass Through Walls");

        return abilities.Count > 0 ? string.Join(", ", abilities) : "None";
    }
}
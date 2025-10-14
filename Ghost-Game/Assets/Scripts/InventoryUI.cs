using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Item Slots")]
    public Image hulaLeiIcon;
    public Image sunShadesIcon;
    public Image partyHatIcon;

    [Header("Item Acquisition Popup")]
    public GameObject itemPopup;
    public Image popupIcon;
    public Text popupItemName;
    public Text popupDescription;
    public float popupDuration = 3f;

    [Header("Persistent Display")]
    public GameObject inventoryPanel;
    public Text abilityListText;

    private float popupTimer = 0f;
    private Dictionary<ItemType, Image> itemSlots = new Dictionary<ItemType, Image>();

    private void Start()
    {
        SetupItemSlots();

        // Hide all icons initially
        SetIconVisibility(hulaLeiIcon, false);
        SetIconVisibility(sunShadesIcon, false);
        SetIconVisibility(partyHatIcon, false);

        if (itemPopup != null)
        {
            itemPopup.SetActive(false);
        }

        // Subscribe to inventory events
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnItemEquipped += OnItemAcquired;
            PlayerInventory.Instance.OnItemRemoved += OnItemRemoved;
        }

        UpdateAbilityList();
    }

    private void SetupItemSlots()
    {
        if (hulaLeiIcon != null)
            itemSlots[ItemType.HulaLei] = hulaLeiIcon;
        if (sunShadesIcon != null)
            itemSlots[ItemType.SunShades] = sunShadesIcon;
        if (partyHatIcon != null)
            itemSlots[ItemType.PartyHat] = partyHatIcon;
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnItemEquipped -= OnItemAcquired;
            PlayerInventory.Instance.OnItemRemoved -= OnItemRemoved;
        }
    }

    private void Update()
    {
        if (popupTimer > 0)
        {
            popupTimer -= Time.deltaTime;

            if (popupTimer <= 0 && itemPopup != null)
            {
                itemPopup.SetActive(false);
            }
        }
    }

    private void OnItemAcquired(Item item)
    {
        if (item == null) return;

        // Update persistent inventory icon
        if (itemSlots.ContainsKey(item.itemType))
        {
            Image icon = itemSlots[item.itemType];
            if (icon != null && item.icon != null)
            {
                icon.sprite = item.icon;
                SetIconVisibility(icon, true);
            }
        }

        // Show acquisition popup
        ShowItemPopup(item);

        // Update ability list
        UpdateAbilityList();
    }

    private void OnItemRemoved(Item item)
    {
        if (item == null) return;

        // Hide inventory icon
        if (itemSlots.ContainsKey(item.itemType))
        {
            Image icon = itemSlots[item.itemType];
            SetIconVisibility(icon, false);
        }

        // Update ability list
        UpdateAbilityList();
    }

    private void ShowItemPopup(Item item)
    {
        if (itemPopup == null) return;

        // Update popup content
        if (popupIcon != null)
        {
            popupIcon.sprite = item.icon;
            popupIcon.enabled = item.icon != null;
        }

        if (popupItemName != null)
        {
            popupItemName.text = item.itemName;
        }

        if (popupDescription != null)
        {
            popupDescription.text = item.description;
        }

        // Show popup
        itemPopup.SetActive(true);
        popupTimer = popupDuration;
    }

    private void UpdateAbilityList()
    {
        if (abilityListText != null && PlayerInventory.Instance != null)
        {
            abilityListText.text = "Abilities: " + PlayerInventory.Instance.GetActiveAbilities();
        }
    }

    private void SetIconVisibility(Image icon, bool visible)
    {
        if (icon != null)
        {
            icon.enabled = visible;

            // Also enable/disable the icon's GameObject if you want
            // icon.gameObject.SetActive(visible);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    [Header("Visibility Settings")]
    public bool startHidden = true;
    public float hiddenAlpha = 0f;
    public float visibleAlpha = 1f;
    public float fadeSpeed = 2f;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Collider2D objectCollider;

    private bool isRevealed = false;
    private float currentAlpha;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();

        if (startHidden)
        {
            SetVisibility(false, true);
        }

        // Subscribe to item equipped event
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnItemEquipped += OnItemEquipped;
        }
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnItemEquipped -= OnItemEquipped;
        }
    }

    private void Update()
    {
        // Check if player has Truth Lens equipped
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.CanSeeHidden())
        {
            if (!isRevealed)
            {
                isRevealed = true;
            }
            UpdateVisibility(true);
        }
        else
        {
            if (isRevealed && startHidden)
            {
                UpdateVisibility(false);
            }
        }
    }

    private void OnItemEquipped(Item item)
    {
        if (item != null && item.canSeeHidden)
        {
            isRevealed = true;
        }
    }

    private void UpdateVisibility(bool visible)
    {
        float targetAlpha = visible ? visibleAlpha : hiddenAlpha;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = color;
            currentAlpha = color.a;
        }

        // Enable/disable collider based on visibility
        if (objectCollider != null)
        {
            objectCollider.enabled = visible;
        }
    }

    private void SetVisibility(bool visible, bool instant = false)
    {
        float targetAlpha = visible ? visibleAlpha : hiddenAlpha;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = targetAlpha;
            spriteRenderer.color = color;
            currentAlpha = targetAlpha;
        }

        if (objectCollider != null)
        {
            objectCollider.enabled = visible;
        }
    }
}

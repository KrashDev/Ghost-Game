using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PhantomWall : MonoBehaviour
{
    [Header("Wall Properties")]
    public bool requiresPhantomCloak = true;

    [Header("Visual Feedback")]
    public SpriteRenderer wallRenderer;
    public Color normalColor = Color.white;
    public Color passableColor = new Color(1f, 1f, 1f, 0.5f);
    public float colorTransitionSpeed = 3f;

    private Collider2D wallCollider;
    private bool isPassable = false;

    private void Start()
    {
        wallCollider = GetComponent<Collider2D>();

        if (wallRenderer == null)
        {
            wallRenderer = GetComponent<SpriteRenderer>();
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
        UpdateWallState();
        UpdateVisual();
    }

    private void UpdateWallState()
    {
        if (!requiresPhantomCloak)
        {
            isPassable = true;
            if (wallCollider != null)
            {
                wallCollider.enabled = false;
            }
            return;
        }

        // Check if player has Phantom Cloak equipped
        bool canPass = PlayerInventory.Instance != null &&
                       PlayerInventory.Instance.CanPassThroughWalls();

        isPassable = canPass;

        if (wallCollider != null)
        {
            wallCollider.enabled = !canPass;
        }
    }

    private void UpdateVisual()
    {
        if (wallRenderer == null) return;

        Color targetColor = isPassable ? passableColor : normalColor;
        wallRenderer.color = Color.Lerp(wallRenderer.color, targetColor,
                                        colorTransitionSpeed * Time.deltaTime);
    }

    private void OnItemEquipped(Item item)
    {
        UpdateWallState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPassable)
        {
            Debug.Log("Passing through phantom wall!");
        }
    }
}
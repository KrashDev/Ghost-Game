using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine;

public class Goblet : MonoBehaviour
{
    [Header("Goblet Settings")]
    [SerializeField] private int gobletIndex; // Unique identifier for this goblet
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite emptyGobletSprite;
    [SerializeField] private Sprite filledGobletSprite;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.Space;
    [SerializeField] private float interactionRange = 2f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    // Event that the manager subscribes to
    public event Action<int> OnGobletClicked;

    private bool isFilled = false;
    private bool playerInRange = false;
    private Transform playerTransform;

    void Awake()
    {
        // Get or add SpriteRenderer
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Get or add AudioSource for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && clickSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Start with empty goblet
        TurnOff();
    }

    void Start()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        // Check if player is in range
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            playerInRange = distance <= interactionRange;
        }

        // Check for spacebar press when player is in range
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        // Toggle to filled state
        TurnOn();

        // Play sound if available
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Notify the manager
        OnGobletClicked?.Invoke(gobletIndex);
    }

    public void TurnOn()
    {
        isFilled = true;
        if (spriteRenderer != null && filledGobletSprite != null)
        {
            spriteRenderer.sprite = filledGobletSprite;
        }
    }

    public void TurnOff()
    {
        isFilled = false;
        if (spriteRenderer != null && emptyGobletSprite != null)
        {
            spriteRenderer.sprite = emptyGobletSprite;
        }
    }

    public bool IsFilled()
    {
        return isFilled;
    }

    public int GetIndex()
    {
        return gobletIndex;
    }

    // For mouse clicks in 2D (alternative to IPointerClickHandler)
    void OnMouseDown()
    {
        // This works if you're not using Unity's Event System
        // Comment this out if using IPointerClickHandler
        /*
        TurnOn();
        
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        
        OnGobletClicked?.Invoke(gobletIndex);
        */
    }
}
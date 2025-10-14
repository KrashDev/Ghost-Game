using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to NPCs or objects that should trigger dialogue
/// Make sure DialogueSystem.cs exists in your Scripts folder!
/// </summary>
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    [SerializeField] private string dialogueText = "Hello! This is a dialogue message.";

    [Header("Settings")]
    [SerializeField] private bool oneTimeOnly = false;

    private DialogueSystem dialogueSystem;
    private bool hasBeenTriggered = false;

    private void Start()
    {
        // Find the dialogue system in the scene
        dialogueSystem = FindObjectOfType<DialogueSystem>();

        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem not found in scene! Please add it to your Canvas.");
        }
    }

    public void Interact()
    {
        // Check if already used
        if (oneTimeOnly && hasBeenTriggered)
        {
            return;
        }

        // Trigger dialogue - ONLY ONE ARGUMENT NOW
        if (dialogueSystem != null)
        {
            dialogueSystem.ShowDialogue(dialogueText);
            hasBeenTriggered = true;
        }
    }
}
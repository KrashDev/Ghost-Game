using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example interactable object - customize this for your game objects
/// </summary>
public class ExampleInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactMessage = "You interacted with this object!";
    [SerializeField] private bool oneTimeUse = false;

    private bool hasBeenUsed = false;

    public void Interact()
    {
        if (oneTimeUse && hasBeenUsed)
        {
            Debug.Log("This object has already been used.");
            return;
        }

        Debug.Log(interactMessage);

        // Add your custom interaction logic here
        // Examples:
        // - Open a door
        // - Pick up an item
        // - Start a dialogue
        // - Activate a mechanism

        if (oneTimeUse)
        {
            hasBeenUsed = true;
        }
    }
}
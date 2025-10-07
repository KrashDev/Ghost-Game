using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for objects that can be interacted with by the player
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player interacts with this object
    /// </summary>
    void Interact();
}
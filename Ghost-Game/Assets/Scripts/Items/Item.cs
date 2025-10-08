using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemType
{
    None,
    HulaLei,      // Walk over holes
    SunShades,       // See hidden things
    PartyHat     // Pass through certain walls
}

[CreateAssetMenu(fileName = "New Item", menuName = "Ghost Game/Item")]
public class Item : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string description;
    public Sprite icon;
    public Sprite characterOverlay; // Visual indicator when worn

    [Header("Ability Properties")]
    public bool canWalkOverHoles;
    public bool canSeeHidden;
    public bool canPassThroughWalls;
}
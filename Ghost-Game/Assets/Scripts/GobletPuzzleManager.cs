using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobletPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [SerializeField] private List<Goblet> goblets = new List<Goblet>();
    [SerializeField] private int[] correctSequence = new int[] { 0, 1, 2, 3 }; // Indices of goblets in correct order
    [SerializeField] private GameObject doorToUnlock;

    [Header("Timing")]
    [SerializeField] private float wrongGobletDisplayTime = 0.5f;

    private List<int> currentSequence = new List<int>();
    private bool puzzleSolved = false;
    private bool isProcessing = false;

    void Start()
    {
        // Subscribe to goblet click events
        foreach (Goblet goblet in goblets)
        {
            goblet.OnGobletClicked += HandleGobletClick;
        }

        // Make sure door is locked at start
        if (doorToUnlock != null)
        {
            doorToUnlock.SetActive(true);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        foreach (Goblet goblet in goblets)
        {
            goblet.OnGobletClicked -= HandleGobletClick;
        }
    }

    private void HandleGobletClick(int gobletIndex)
    {
        // Don't process if puzzle is solved or currently processing
        if (puzzleSolved || isProcessing)
            return;

        // Add to current sequence
        currentSequence.Add(gobletIndex);

        // Check if this click is correct so far
        int currentStep = currentSequence.Count - 1;

        if (currentSequence[currentStep] == correctSequence[currentStep])
        {
            // Correct so far
            Debug.Log($"Correct! Step {currentStep + 1} of {correctSequence.Length}");

            // Check if puzzle is complete
            if (currentSequence.Count == correctSequence.Length)
            {
                StartCoroutine(PuzzleSolved());
            }
        }
        else
        {
            // Wrong goblet clicked
            Debug.Log("Wrong goblet! Resetting...");
            StartCoroutine(HandleWrongGoblet(gobletIndex));
        }
    }

    private IEnumerator HandleWrongGoblet(int wrongGobletIndex)
    {
        isProcessing = true;

        // Wait for the display time
        yield return new WaitForSeconds(wrongGobletDisplayTime);

        // Turn off the wrong goblet
        goblets[wrongGobletIndex].TurnOff();

        // Reset all other goblets that were activated
        foreach (int index in currentSequence)
        {
            if (index != wrongGobletIndex)
            {
                goblets[index].TurnOff();
            }
        }

        // Clear the sequence
        currentSequence.Clear();

        isProcessing = false;
    }

    private IEnumerator PuzzleSolved()
    {
        puzzleSolved = true;
        Debug.Log("Puzzle Solved! Door unlocked!");

        // Optional: Add celebration effects here
        yield return new WaitForSeconds(0.5f);

        // Unlock the door
        if (doorToUnlock != null)
        {
            doorToUnlock.SetActive(false);
            // Or you could trigger an animation, move it, etc.
        }
    }

    // Optional: Method to reset puzzle manually
    public void ResetPuzzle()
    {
        if (puzzleSolved)
            return;

        StopAllCoroutines();
        currentSequence.Clear();
        isProcessing = false;

        foreach (Goblet goblet in goblets)
        {
            goblet.TurnOff();
        }
    }
}
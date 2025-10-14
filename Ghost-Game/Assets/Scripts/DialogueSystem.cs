using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Settings")]
    [SerializeField] private float textSpeed = 0.05f;

    private KeyCode advanceKey = KeyCode.Space; // Hardcoded to spacebar
    private bool isTyping = false;
    private bool dialogueActive = false;
    private Coroutine typingCoroutine;
    private string fullText = "";

    private void Start()
    {
        // Hide dialogue box at start
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueActive) return;

        // Press space to skip typing or close dialogue
        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping)
            {
                // Skip to end of current text
                SkipTyping();
            }
            else
            {
                // Close dialogue
                EndDialogue();
            }
        }
    }

    /// <summary>
    /// Show dialogue with specified text
    /// </summary>
    public void ShowDialogue(string text)
    {
        // Error checking
        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel is not assigned in DialogueSystem!");
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogError("DialogueText is not assigned in DialogueSystem!");
            return;
        }

        dialogueActive = true;
        dialoguePanel.SetActive(true);
        fullText = text;

        // Start typing effect
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    /// <summary>
    /// Type text letter by letter
    /// </summary>
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    /// <summary>
    /// Skip typing animation and show full text
    /// </summary>
    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        dialogueText.text = fullText;
    }

    /// <summary>
    /// End and hide dialogue
    /// </summary>
    public void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    /// <summary>
    /// Check if dialogue is currently active
    /// </summary>
    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueIndicator;

    [Header("Dialogue Settings")]
    [SerializeField] private float textSpeed = 0.05f;

    private KeyCode advanceKey = KeyCode.Space; // Hardcoded to spacebar

    private bool isTyping = false;
    private bool dialogueActive = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        // Hide dialogue box at start
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueActive) return;

        // Advance dialogue or skip typing
        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping)
            {
                // Skip to end of current text
                SkipTyping();
            }
            else
            {
                // Close dialogue (or advance to next line in a queue system)
                EndDialogue();
            }
        }
    }

    /// <summary>
    /// Show dialogue with specified speaker name and text
    /// </summary>
    public void ShowDialogue(string speakerName, string text)
    {
        dialogueActive = true;
        dialoguePanel.SetActive(true);

        // Set speaker name
        if (nameText != null)
            nameText.text = speakerName;

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

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        if (continueIndicator != null)
            continueIndicator.SetActive(true);
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

        if (continueIndicator != null)
            continueIndicator.SetActive(true);
    }

    /// <summary>
    /// End and hide dialogue
    /// </summary>
    public void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        nameText.text = "";
    }

    /// <summary>
    /// Check if dialogue is currently active
    /// </summary>
    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}
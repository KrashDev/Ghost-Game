using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionUI : MonoBehaviour
{
    [Header("Character Buttons")]
    public Button character1Button;
    public Button character2Button;
    public Button character3Button;

    [Header("Character Displays (Optional)")]
    public Image character1Image;
    public Image character2Image;
    public Image character3Image;

    void Start()
    {
        // Setup button listeners - clicking immediately loads the game
        character1Button.onClick.AddListener(() => SelectCharacterAndPlay(0));
        character2Button.onClick.AddListener(() => SelectCharacterAndPlay(1));
        character3Button.onClick.AddListener(() => SelectCharacterAndPlay(2));

        // Display character sprites if image references are assigned
        DisplayCharacters();
    }

    void DisplayCharacters()
    {
        if (GameManager.Instance == null || GameManager.Instance.characters == null)
        {
            Debug.LogError("GameManager not found or characters not assigned!");
            return;
        }

        // Show character sprites on the buttons
        if (character1Image != null && GameManager.Instance.characters.Length > 0)
            character1Image.sprite = GameManager.Instance.characters[0].characterSprite;

        if (character2Image != null && GameManager.Instance.characters.Length > 1)
            character2Image.sprite = GameManager.Instance.characters[1].characterSprite;

        if (character3Image != null && GameManager.Instance.characters.Length > 2)
            character3Image.sprite = GameManager.Instance.characters[2].characterSprite;
    }

    void SelectCharacterAndPlay(int characterIndex)
    {
        // Save the selected character
        GameManager.Instance.selectedCharacterIndex = characterIndex;

        // Immediately load the game scene
        SceneManager.LoadScene("Game");
    }
}
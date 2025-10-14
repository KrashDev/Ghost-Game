using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Settings")]
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private int minNameLength = 2;
    [SerializeField] private int maxNameLength = 20;

    private void Start()
    {
        // Setup button listener
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        // Setup input field listener
        if (nameInputField != null)
        {
            nameInputField.characterLimit = maxNameLength;
            nameInputField.onValueChanged.AddListener(OnNameInputChanged);

            // Load previous name if exists
            string savedName = PlayerPrefs.GetString("PlayerName", "");
            if (!string.IsNullOrEmpty(savedName))
            {
                nameInputField.text = savedName;
            }
        }

        // Hide error text initially
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }

        // Create PlayerDataManager if it doesn't exist
        if (PlayerDataManager.Instance == null)
        {
            GameObject manager = new GameObject("PlayerDataManager");
            manager.AddComponent<PlayerDataManager>();
        }
    }

    private void OnNameInputChanged(string value)
    {
        // Hide error when user starts typing
        if (errorText != null && errorText.gameObject.activeSelf)
        {
            errorText.gameObject.SetActive(false);
        }
    }

    private void OnStartButtonClicked()
    {
        string playerName = nameInputField.text.Trim();

        // Validate name
        if (string.IsNullOrEmpty(playerName))
        {
            ShowError("Please enter your name!");
            return;
        }

        if (playerName.Length < minNameLength)
        {
            ShowError($"Name must be at least {minNameLength} characters!");
            return;
        }

        // Save player name
        PlayerDataManager.Instance.SetPlayerName(playerName);

        // Load game scene
        StartGame();
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }
        Debug.LogWarning(message);
    }

    private void StartGame()
    {
        Debug.Log($"Starting game with player: {nameInputField.text}");
        SceneManager.LoadScene(gameSceneName);
    }

    // Optional: Allow Enter key to submit
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnStartButtonClicked();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameFinishUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI finishTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private GameObject newRecordBanner;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Settings")]
    [SerializeField] private string startSceneName = "StartMenu";
    [SerializeField] private Color newRecordColor = Color.yellow;

    private void Start()
    {
        // Hide finish panel initially
        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
        }

        // Setup button listeners
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }

    public void ShowFinishUI(float completionTime)
    {
        if (finishPanel == null)
        {
            Debug.LogError("Finish panel is not assigned!");
            return;
        }

        // Get player data
        string playerName = "Player";
        if (PlayerDataManager.Instance != null)
        {
            playerName = PlayerDataManager.Instance.PlayerName;
            PlayerDataManager.Instance.CurrentRunTime = completionTime;
            PlayerDataManager.Instance.SaveRunData();
        }

        // Check if it's a new record
        float previousBestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        bool isNewRecord = completionTime < previousBestTime && previousBestTime != float.MaxValue;

        // Update UI texts
        UpdateFinishTexts(playerName, completionTime, isNewRecord);

        // Show the finish panel
        finishPanel.SetActive(true);

        // Optional: Pause the game
        // Time.timeScale = 0f;
    }

    private void UpdateFinishTexts(string playerName, float completionTime, bool isNewRecord)
    {
        // Congratulations message
        if (congratsText != null)
        {
            congratsText.text = "Level Complete!";
        }

        // Player name
        if (playerNameText != null)
        {
            playerNameText.text = $"{playerName}";
        }

        // Finish time
        if (finishTimeText != null)
        {
            finishTimeText.text = $"{FormatTime(completionTime)}";

            // Highlight if new record
            if (isNewRecord)
            {
                finishTimeText.color = newRecordColor;
            }
        }

        // Best time
        if (bestTimeText != null)
        {
            float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
            string bestPlayer = PlayerPrefs.GetString("BestTimePlayer", "Unknown");

            if (bestTime == float.MaxValue)
            {
                bestTimeText.text = "Best: None";
            }
            else
            {
                bestTimeText.text = $"Best: {FormatTime(bestTime)} by {bestPlayer}";
            }
        }

        // New record banner
        if (newRecordBanner != null)
        {
            newRecordBanner.SetActive(isNewRecord);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    private void OnPlayAgainClicked()
    {
        // Unpause if needed
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMainMenuClicked()
    {
        // Unpause if needed
        Time.timeScale = 1f;

        // Load start menu
        SceneManager.LoadScene(startSceneName);
    }

    public void HideFinishUI()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
        }
    }
}
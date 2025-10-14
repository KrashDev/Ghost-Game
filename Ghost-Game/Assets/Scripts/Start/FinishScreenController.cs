using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FinishScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI finishTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI newRecordText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Settings")]
    [SerializeField] private string startSceneName = "StartMenu";
    [SerializeField] private bool showNewRecordMessage = true;

    private bool isNewRecord = false;

    private void Start()
    {
        // Hide finish panel initially
        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
        }

        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        // Hide new record text initially
        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(false);
        }
    }

    public void ShowFinishScreen(float completionTime)
    {
        if (finishPanel == null) return;

        // Get player data
        string playerName = PlayerDataManager.Instance != null
            ? PlayerDataManager.Instance.PlayerName
            : PlayerPrefs.GetString("PlayerName", "Player");

        // Save the run data
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.CurrentRunTime = completionTime;
            PlayerDataManager.Instance.SaveRunData();
        }

        // Check if it's a new record
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        isNewRecord = completionTime < bestTime && bestTime != float.MaxValue;

        // Display player name
        if (playerNameText != null)
        {
            playerNameText.text = $"Congratulations, {playerName}!";
        }

        // Display finish time
        if (finishTimeText != null)
        {
            finishTimeText.text = $"Your Time: {FormatTime(completionTime)}";
        }

        // Display best time
        if (bestTimeText != null)
        {
            if (bestTime == float.MaxValue)
            {
                bestTimeText.text = "Best Time: --:--.--";
            }
            else
            {
                string bestPlayer = PlayerPrefs.GetString("BestTimePlayer", "Unknown");
                bestTimeText.text = $"Best Time: {FormatTime(bestTime)} ({bestPlayer})";
            }
        }

        // Show new record message if applicable
        if (newRecordText != null && showNewRecordMessage && isNewRecord)
        {
            newRecordText.gameObject.SetActive(true);
        }

        // Show the finish panel
        finishPanel.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    private void OnRestartClicked()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMainMenuClicked()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Load start menu
        SceneManager.LoadScene(startSceneName);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timer State")]
    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool hasFinished = false;

    // Public property to access final time
    public float FinalTime { get; private set; }

    private void Awake()
    {
        if (startOnAwake)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        isRunning = true;
        elapsedTime = 0f;
        hasFinished = false;
    }

    public void StopTimer()
    {
        if (!hasFinished)
        {
            isRunning = false;
            hasFinished = true;
            FinalTime = elapsedTime;

            // Save the time
            SaveTime();

            Debug.Log($"Timer stopped at: {FormatTime(FinalTime)}");
        }
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        if (!hasFinished)
        {
            isRunning = true;
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(elapsedTime);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    private void SaveTime()
    {
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat("LastCompletionTime", FinalTime);

        // Check and save best time
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (FinalTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", FinalTime);
            Debug.Log($"New best time: {FormatTime(FinalTime)}");
        }

        PlayerPrefs.Save();
    }

    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat("BestTime", float.MaxValue);
    }

    public string GetBestTimeFormatted()
    {
        float bestTime = GetBestTime();
        if (bestTime == float.MaxValue)
        {
            return "No Record";
        }
        return FormatTime(bestTime);
    }

    public float GetCurrentTime()
    {
        return elapsedTime;
    }
}
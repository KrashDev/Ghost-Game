using UnityEngine.SceneManagement;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private InGameFinishUI finishUI;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool goalReached = false;

    private void Awake()
    {
        // Find the timer if not assigned
        if (gameTimer == null)
        {
            gameTimer = FindObjectOfType<GameTimer>();
        }

        // Find the finish UI if not assigned
        if (finishUI == null)
        {
            finishUI = FindObjectOfType<InGameFinishUI>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!goalReached && other.CompareTag(playerTag))
        {
            ReachGoal();
        }
    }

    private void ReachGoal()
    {
        goalReached = true;

        // Stop the timer
        float finalTime = 0f;
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
            finalTime = gameTimer.FinalTime;
            Debug.Log($"Goal reached! Final time: {finalTime}");
        }

        // Show finish UI
        if (finishUI != null)
        {
            finishUI.ShowFinishUI(finalTime);
        }
        else
        {
            Debug.LogWarning("Finish UI not found!");
        }
    }
}
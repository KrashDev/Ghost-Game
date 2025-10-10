using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameTimer gameTimer;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool reloadSceneOnComplete = false;
    [SerializeField] private float reloadDelay = 2f;

    private bool goalReached = false;

    private void Awake()
    {
        // Find the timer if not assigned
        if (gameTimer == null)
        {
            gameTimer = FindObjectOfType<GameTimer>();
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
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
            Debug.Log($"Goal reached! Final time: {gameTimer.FinalTime}");
        }

        // Optional: Reload scene
        if (reloadSceneOnComplete)
        {
            Invoke(nameof(ReloadScene), reloadDelay);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
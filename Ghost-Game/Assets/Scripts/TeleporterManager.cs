using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class TeleporterManager : MonoBehaviour
{
    public static TeleporterManager Instance { get; private set; }

    [Header("Sequence Settings")]
    [SerializeField] private int currentSequenceStep = 0;
    [SerializeField] private int totalSequenceSteps = 3;
    [SerializeField] private bool loopSequence = false;

    [Header("Completion Settings")]
    [SerializeField] private GameObject goalObject;
    [SerializeField] private bool unlockGoalOnComplete = true;

    [Header("Events")]
    public UnityEvent onSequenceAdvanced;
    public UnityEvent onSequenceReset;
    public UnityEvent onSequenceCompleted;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private List<Teleporter> registeredTeleporters = new List<Teleporter>();
    private bool sequenceCompleted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (goalObject != null)
        {
            goalObject.SetActive(!unlockGoalOnComplete);
        }

        UpdateAllTeleporterVisuals();
    }

    public void RegisterTeleporter(Teleporter teleporter)
    {
        if (!registeredTeleporters.Contains(teleporter))
        {
            registeredTeleporters.Add(teleporter);

            if (showDebugInfo)
            {
                Debug.Log($"Registered teleporter {teleporter.GetTeleporterID()} " +
                         $"(Sequence: {teleporter.GetRequiredSequenceNumber()})");
            }
        }
    }

    public void AdvanceSequence()
    {
        if (sequenceCompleted && !loopSequence)
        {
            return;
        }

        currentSequenceStep++;

        if (showDebugInfo)
        {
            Debug.Log($"Sequence advanced to step {currentSequenceStep}/{totalSequenceSteps}");
        }

        onSequenceAdvanced?.Invoke();

        if (currentSequenceStep >= totalSequenceSteps)
        {
            CompleteSequence();
        }

        UpdateAllTeleporterVisuals();
    }

    public void ResetSequence()
    {
        currentSequenceStep = 0;
        sequenceCompleted = false;

        if (showDebugInfo)
        {
            Debug.Log("Sequence reset!");
        }

        onSequenceReset?.Invoke();
        UpdateAllTeleporterVisuals();
    }

    private void CompleteSequence()
    {
        sequenceCompleted = true;

        if (showDebugInfo)
        {
            Debug.Log("Sequence completed!");
        }

        if (unlockGoalOnComplete && goalObject != null)
        {
            goalObject.SetActive(true);
        }

        onSequenceCompleted?.Invoke();

        if (loopSequence)
        {
            currentSequenceStep = 0;
        }
    }

    public int GetCurrentSequence() => currentSequenceStep;

    public bool IsSequenceCompleted() => sequenceCompleted;

    public void UpdateAllTeleporterVisuals()
    {
        foreach (var teleporter in registeredTeleporters)
        {
            if (teleporter != null)
            {
                teleporter.UpdateVisualState();
            }
        }
    }

    public void SetTotalSequenceSteps(int steps)
    {
        totalSequenceSteps = steps;
    }

    private void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Box($"Teleporter Sequence: {currentSequenceStep}/{totalSequenceSteps}");
            GUILayout.Box($"Status: {(sequenceCompleted ? "COMPLETED" : "IN PROGRESS")}");

            if (GUILayout.Button("Reset Sequence"))
            {
                ResetSequence();
            }

            GUILayout.EndArea();
        }
    }
}
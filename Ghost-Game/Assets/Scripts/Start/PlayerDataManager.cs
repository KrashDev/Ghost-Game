using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class to persist player data across scenes
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    // Player Data
    public string PlayerName { get; private set; }
    public float CurrentRunTime { get; set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
        Debug.Log($"Player name set to: {name}");
    }

    public void LoadPlayerName()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", "Player");
    }

    public void SaveRunData()
    {
        PlayerPrefs.SetFloat("LastRunTime", CurrentRunTime);
        PlayerPrefs.SetString("LastRunPlayer", PlayerName);

        // Check for best time
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (CurrentRunTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", CurrentRunTime);
            PlayerPrefs.SetString("BestTimePlayer", PlayerName);
        }

        PlayerPrefs.Save();
        Debug.Log($"Run data saved: {PlayerName} - {CurrentRunTime}s");
    }

    public string GetBestTimePlayer()
    {
        return PlayerPrefs.GetString("BestTimePlayer", "None");
    }

    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat("BestTime", float.MaxValue);
    }
}
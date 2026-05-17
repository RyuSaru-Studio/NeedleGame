using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    private static LevelProgressManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static LevelProgressManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObject = new GameObject("LevelProgressManager");
                instance = managerObject.AddComponent<LevelProgressManager>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Saves stars for a level only if they are better than the previous best.
    /// </summary>
    public void SaveStars(string levelName, int starCount)
    {
        int currentBest = LoadStars(levelName);
        
        if (starCount > currentBest)
        {
            string key = "LevelStars_" + levelName;
            PlayerPrefs.SetInt(key, starCount);
            PlayerPrefs.Save();
            Debug.Log($"Saved {starCount} stars for level '{levelName}'");
        }
        else if (starCount == currentBest)
        {
            Debug.Log($"Level '{levelName}' already has {currentBest} stars. No update needed.");
        }
        else
        {
            Debug.Log($"New score ({starCount} stars) is worse than best ({currentBest} stars). Not saving.");
        }
    }

    /// <summary>
    /// Loads the best star count for a level. Returns 0 if never played.
    /// </summary>
    public int LoadStars(string levelName)
    {
        string key = "LevelStars_" + levelName;
        return PlayerPrefs.GetInt(key, 0);
    }

    /// <summary>
    /// Gets the best star count for a level.
    /// </summary>
    public int GetBestStars(string levelName)
    {
        return LoadStars(levelName);
    }

    /// <summary>
    /// Deletes the saved stars for a single level.
    /// </summary>
    public void DeleteStars(string levelName)
    {
        string key = "LevelStars_" + levelName;
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log($"Deleted saved stars for level '{levelName}'");
        }
    }

    /// <summary>
    /// Clears the saved stars for the given level names.
    /// </summary>
    public void ClearStars(string[] levelNames)
    {
        if (levelNames == null)
        {
            return;
        }

        foreach (string levelName in levelNames)
        {
            if (string.IsNullOrEmpty(levelName))
            {
                continue;
            }

            string key = "LevelStars_" + levelName;
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                Debug.Log($"Deleted saved stars for level '{levelName}'");
            }
        }

        PlayerPrefs.Save();
        Debug.Log("Cleared saved stars for listed levels.");
    }

    /// <summary>
    /// Clears all level progress data. (For testing/reset)
    /// </summary>
    public void ClearAllProgress()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All level progress cleared.");
    }
}
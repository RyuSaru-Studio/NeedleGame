using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private int perfectPoints = 10;
    [SerializeField] private int goodPoints = 5;
    [SerializeField] private int missPoints = 0;

    private int currentScore;
    private int maximumPossibleScore;

    public int CurrentScore => currentScore;
    public int MaximumPossibleScore => maximumPossibleScore;

    private void OnValidate()
    {
        perfectPoints = Mathf.Max(0, perfectPoints);
        goodPoints = Mathf.Max(0, goodPoints);
        missPoints = Mathf.Max(0, missPoints);
    }

    /// <summary>
    /// Sets the maximum possible score for this level based on hole count.
    /// Max = holesCount * perfectPoints
    /// </summary>
    public void SetMaximumScore(int holesCount)
    {
        maximumPossibleScore = holesCount * perfectPoints;
        Debug.Log($"Maximum score set to {maximumPossibleScore} ({holesCount} holes × {perfectPoints} points)");
    }

    /// <summary>
    /// Returns the current score as a percentage of the maximum possible score.
    /// </summary>
    public float GetScorePercentage()
    {
        if (maximumPossibleScore <= 0)
        {
            return 0f;
        }

        return (currentScore / (float)maximumPossibleScore) * 100f;
    }

    public void ResetScore()
    {
        currentScore = 0;
        Debug.Log($"Score: {currentScore}");
    }

    public void AddScoreForResult(TimingResult result)
    {
        int pointsToAdd = 0;

        if (result == TimingResult.Perfect)
        {
            pointsToAdd = perfectPoints;
        }
        else if (result == TimingResult.Good)
        {
            pointsToAdd = goodPoints;
        }
        else if (result == TimingResult.Miss)
        {
            pointsToAdd = missPoints;
        }

        currentScore += pointsToAdd;

        if (pointsToAdd != 0)
        {
            Debug.Log($"Score: {currentScore} ({GetScorePercentage():F1}%)");
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private int perfectPoints = 100;
    [SerializeField] private int goodPoints = 50;
    [SerializeField] private int missPoints = 0;

    private int currentScore;

    public int CurrentScore => currentScore;

    private void OnValidate()
    {
        perfectPoints = Mathf.Max(0, perfectPoints);
        goodPoints = Mathf.Max(0, goodPoints);
        missPoints = Mathf.Max(0, missPoints);
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
            Debug.Log($"Score: {currentScore}");
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}

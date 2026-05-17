using TMPro;
using UnityEngine;

public class StarRatingController : MonoBehaviour
{
    public ScoreController scoreController;
    public TextMeshProUGUI starsText;

    private int currentStarCount;
    private int displayedStarCount; // For UI animation

    public int CurrentStarCount => currentStarCount;

    public void ResetStars()
    {
        currentStarCount = 0;
        displayedStarCount = 0;
        UpdateStarsText();
    }

    /// <summary>
    /// Calculates stars based on percentage of perfect actions.
    /// 100% = 3 stars
    /// 70% to 99.9% = 2 stars
    /// Below 70% = 1 star
    /// </summary>
    public int CalculateStarsByPercentage(float percentage)
    {
        if (percentage >= 100f)
        {
            return 3;
        }

        if (percentage >= 70f)
        {
            return 2;
        }

        if (percentage >= 1f) // At least some points
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Updates current star count based on the current score.
    /// </summary>
    public void UpdateStarsFromScore()
    {
        if (scoreController == null)
        {
            return;
        }

        float percentage = scoreController.GetScorePercentage();
        currentStarCount = CalculateStarsByPercentage(percentage);
        UpdateStarsText();
    }

    public void ShowStarsForScore(int score)
    {
        currentStarCount = CalculateStarsByPercentage((score / (float)scoreController.MaximumPossibleScore) * 100f);
        UpdateStarsText();
    }

    public int GetCurrentStarCount()
    {
        return currentStarCount;
    }

    private void UpdateStarsText()
    {
        if (starsText == null)
        {
            return;
        }

        if (currentStarCount == 3)
        {
            starsText.text = "★★★";
        }
        else if (currentStarCount == 2)
        {
            starsText.text = "★★☆";
        }
        else if (currentStarCount == 1)
        {
            starsText.text = "★☆☆";
        }
        else
        {
            starsText.text = "☆☆☆";
        }
    }
}
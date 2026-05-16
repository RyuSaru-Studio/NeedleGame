using TMPro;
using UnityEngine;

public class StarRatingController : MonoBehaviour
{
    public ScoreController scoreController;
    public TextMeshProUGUI starsText;
    public int oneStarThreshold = 150;
    public int twoStarThreshold = 250;
    public int threeStarThreshold = 350;

    private int currentStarCount;

    public int CurrentStarCount => currentStarCount;

    private void OnValidate()
    {
        oneStarThreshold = Mathf.Max(0, oneStarThreshold);
        twoStarThreshold = Mathf.Max(oneStarThreshold, twoStarThreshold);
        threeStarThreshold = Mathf.Max(twoStarThreshold, threeStarThreshold);
    }

    public void ResetStars()
    {
        currentStarCount = 0;
        UpdateStarsText();
    }

    public int CalculateStars(int score)
    {
        if (score >= threeStarThreshold)
        {
            return 3;
        }

        if (score >= twoStarThreshold)
        {
            return 2;
        }

        if (score >= oneStarThreshold)
        {
            return 1;
        }

        return 0;
    }

    public void ShowStarsForScore(int score)
    {
        currentStarCount = CalculateStars(score);
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
            starsText.text = "\u2605\u2605\u2605";
        }
        else if (currentStarCount == 2)
        {
            starsText.text = "\u2605\u2605\u2606";
        }
        else if (currentStarCount == 1)
        {
            starsText.text = "\u2605\u2606\u2606";
        }
        else
        {
            starsText.text = "\u2606\u2606\u2606";
        }
    }
}

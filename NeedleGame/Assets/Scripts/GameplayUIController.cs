using TMPro;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI hintText;

    public void ShowResult(TimingResult result)
    {
        if (resultText == null)
        {
            return;
        }

        if (result == TimingResult.Perfect)
        {
            resultText.text = "PERFECT!";
        }
        else if (result == TimingResult.Good)
        {
            resultText.text = "GOOD!";
        }
        else
        {
            resultText.text = "MISS!";
        }
    }

    public void UpdateProgress(int currentHoleIndex, int totalHoles)
    {
        if (progressText == null)
        {
            return;
        }

        progressText.text = $"Hole {currentHoleIndex + 1} / {totalHoles}";
    }

    public void ShowLevelComplete()
    {
        if (resultText != null)
        {
            resultText.text = "REPAIRED!";
        }

        ShowHint("Press R to restart");
    }

    public void ShowHint(string message)
    {
        if (hintText != null)
        {
            hintText.text = message;
        }
    }

    public void ClearResult()
    {
        if (resultText != null)
        {
            resultText.text = "";
        }
    }
}

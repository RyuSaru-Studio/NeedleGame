using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenuButton : MonoBehaviour
{
    public TextMeshProUGUI levelNameText;
    public Button levelButton;
    public StarDisplayUI starDisplayUI;
    private string sceneName;

    public void Setup(string scene)
    {
        sceneName = scene;
        if (levelNameText != null) levelNameText.text = scene;
        int bestStars = LevelProgressManager.Instance != null ? LevelProgressManager.Instance.LoadStars(scene) : 0;
        if (starDisplayUI != null) starDisplayUI.UpdateStarDisplay(bestStars);

        if (levelButton != null)
        {
            levelButton.onClick.RemoveAllListeners();
            levelButton.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayClick();
                SceneManager.LoadScene(sceneName);
            });
        }
    }

    // Optional: refresh displayed stars (call when returning to menu)
    public void Refresh()
    {
        int bestStars = LevelProgressManager.Instance != null ? LevelProgressManager.Instance.LoadStars(sceneName) : 0;
        if (starDisplayUI != null) starDisplayUI.UpdateStarDisplay(bestStars);
    }
}
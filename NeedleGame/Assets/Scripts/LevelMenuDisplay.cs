using UnityEngine;
using UnityEngine.UI;

public class LevelMenuDisplay : MonoBehaviour
{
    [Tooltip("Scene names to show in the level menu")]
    public string[] levelSceneNames;
    public Transform contentParent; // container for instanced buttons (e.g., Vertical Layout)
    public GameObject levelButtonPrefab; // prefab with LevelMenuButton script
    public Button resetStarsButton;

    private void OnEnable()
    {
        PopulateLevels();
        SetupResetButton();
    }

    private void SetupResetButton()
    {
        if (resetStarsButton == null)
            return;

        resetStarsButton.onClick.RemoveAllListeners();
        resetStarsButton.onClick.AddListener(() => { AudioManager.Instance?.PlayClick(); ResetAllStars(); });
    }

    [ContextMenu("Populate Levels")]
    public void PopulateLevels()
    {
        if (contentParent == null || levelButtonPrefab == null || levelSceneNames == null) return;

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(contentParent.GetChild(i).gameObject);

        foreach (var scene in levelSceneNames)
        {
            if (string.IsNullOrEmpty(scene)) continue;
            var go = Instantiate(levelButtonPrefab, contentParent);
            var btn = go.GetComponent<LevelMenuButton>();
            if (btn != null) btn.Setup(scene);
        }
    }

    public void ResetAllStars()
    {
        if (levelSceneNames == null || levelSceneNames.Length == 0)
            return;

        LevelProgressManager.Instance.ClearStars(levelSceneNames);
        PopulateLevels();
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RepairCompletionController : MonoBehaviour
{
    public HoleSequenceController holeSequenceController;
    public ScoreController scoreController;
    public StarRatingController starRatingController;
    public LevelUIController levelUIController;
    public StitchingFeedbackLine stitchingFeedbackLine;
    public SpriteRenderer clothingRenderer;
    public Sprite repairedClothingSprite;
    public SpriteRenderer ripOverlay;
    public List<Transform> holeTransforms = new List<Transform>();
    public List<Transform> beautifulSeamTargets = new List<Transform>();
    public GameObject levelCompletePanel;
    public StarDisplayUI endscreenStarDisplay;
    public TextMeshProUGUI completeStarsText;
    public TextMeshProUGUI completeScoreText;
    public TextMeshProUGUI completeHintText;
    public float repairStartDelay = 0.25f;
    public float seamPolishDuration = 0.45f;
    public float showPanelDelay = 0.2f;

    private Coroutine completionRoutine;
    private bool hasCompleted;

    private void Awake()
    {
        AutoAssignEndscreenReferences();
        AutoAssignLevelUIController();

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (holeSequenceController != null)
        {
            holeSequenceController.OnLevelCompleted.AddListener(HandleLevelCompleted);
        }
    }

    private void OnDisable()
    {
        if (holeSequenceController != null)
        {
            holeSequenceController.OnLevelCompleted.RemoveListener(HandleLevelCompleted);
        }

        if (completionRoutine != null)
        {
            StopCoroutine(completionRoutine);
            completionRoutine = null;
        }
    }

    private void OnValidate()
    {
        repairStartDelay = Mathf.Max(0f, repairStartDelay);
        seamPolishDuration = Mathf.Max(0f, seamPolishDuration);
        showPanelDelay = Mathf.Max(0f, showPanelDelay);
    }

    private void HandleLevelCompleted()
    {
        if (hasCompleted)
        {
            return;
        }

        hasCompleted = true;
        completionRoutine = StartCoroutine(PlayRepairCompletionSequence());
    }

    private IEnumerator PlayRepairCompletionSequence()
    {
        if (repairStartDelay > 0f)
        {
            yield return new WaitForSeconds(repairStartDelay);
        }

        ShowRepairedClothing();
        yield return PolishSeam();

        if (showPanelDelay > 0f)
        {
            yield return new WaitForSeconds(showPanelDelay);
        }

        ShowLevelCompletePanel();

        if (levelUIController != null)
        {
            levelUIController.CompleteLevel();
        }
        else
        {
            Debug.LogWarning("RepairCompletionController: LevelUIController is not assigned. Endscreen music will not play.");
            AutoAssignLevelUIController();
            if (levelUIController != null)
            {
                levelUIController.CompleteLevel();
            }
        }

        completionRoutine = null;
    }

    private void ShowRepairedClothing()
    {
        if (clothingRenderer != null && repairedClothingSprite != null)
        {
            clothingRenderer.sprite = repairedClothingSprite;
        }

        if (ripOverlay != null)
        {
            ripOverlay.enabled = false;
        }
    }

    private IEnumerator PolishSeam()
    {
        if (!HasValidSeamTargets())
        {
            Debug.LogWarning("RepairCompletionController could not polish the seam because holeTransforms and beautifulSeamTargets are missing or do not match.");

            if (stitchingFeedbackLine != null)
            {
                stitchingFeedbackLine.SetTailVisible(false);
            }

            yield break;
        }

        if (stitchingFeedbackLine != null)
        {
            stitchingFeedbackLine.SetTailVisible(false);
        }

        List<Vector3> startPositions = new List<Vector3>(holeTransforms.Count);

        for (int i = 0; i < holeTransforms.Count; i++)
        {
            startPositions.Add(holeTransforms[i].position);
        }

        if (seamPolishDuration <= 0f)
        {
            MoveHolesToTargets(1f, startPositions);
            UpdateCompletedThreadLine();
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < seamPolishDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / seamPolishDuration);
            MoveHolesToTargets(Mathf.SmoothStep(0f, 1f, t), startPositions);
            UpdateCompletedThreadLine();
            yield return null;
        }

        MoveHolesToTargets(1f, startPositions);
        UpdateCompletedThreadLine();
    }

    private bool HasValidSeamTargets()
    {
        if (holeTransforms == null || beautifulSeamTargets == null || holeTransforms.Count == 0 || holeTransforms.Count != beautifulSeamTargets.Count)
        {
            return false;
        }

        for (int i = 0; i < holeTransforms.Count; i++)
        {
            if (holeTransforms[i] == null || beautifulSeamTargets[i] == null)
            {
                return false;
            }
        }

        return true;
    }

    private void MoveHolesToTargets(float t, List<Vector3> startPositions)
    {
        for (int i = 0; i < holeTransforms.Count; i++)
        {
            holeTransforms[i].position = Vector3.Lerp(startPositions[i], beautifulSeamTargets[i].position, t);
        }
    }

    private void UpdateCompletedThreadLine()
    {
        if (stitchingFeedbackLine == null)
        {
            return;
        }

        stitchingFeedbackLine.SetTailVisible(false);
        stitchingFeedbackLine.RebuildCompletedThreadFromTransforms(holeTransforms);
    }

    private void ShowLevelCompletePanel()
    {
        AutoAssignEndscreenReferences();

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        int finalScore = scoreController != null ? scoreController.CurrentScore : 0;
        int starCount = GetFinalStarCount(finalScore);

        Debug.Log($"RepairCompletionController.ShowLevelCompletePanel: finalScore={finalScore}, starCount={starCount}, endscreenStarDisplay={(endscreenStarDisplay != null)}");

        if (endscreenStarDisplay != null)
        {
            endscreenStarDisplay.UpdateStarDisplay(starCount);
        }

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        LevelProgressManager.Instance.SaveStars(sceneName, starCount);

        if (completeScoreText != null)
        {
            completeScoreText.text = $"Score: {finalScore}";
        }

        if (completeStarsText != null)
        {
            completeStarsText.text = GetStarText(starCount);
        }

        if (completeHintText != null)
        {
            completeHintText.text = "Press R to restart";
        }
    }

    private int GetFinalStarCount(int finalScore)
    {
        if (starRatingController == null)
        {
            return 0;
        }

        starRatingController.ShowStarsForScore(finalScore);
        return starRatingController.CurrentStarCount;
    }

    private string GetStarText(int starCount)
    {
        if (starCount >= 3)
        {
            return "\u2605\u2605\u2605";
        }

        if (starCount == 2)
        {
            return "\u2605\u2605\u2606";
        }

        if (starCount == 1)
        {
            return "\u2605\u2606\u2606";
        }

        return "\u2606\u2606\u2606";
    }

    private void AutoAssignEndscreenReferences()
    {
        if (levelCompletePanel == null)
        {
            levelCompletePanel = FindSceneObjectByName("EndscreenPanel");
        }

        if (levelCompletePanel == null)
        {
            return;
        }

        completeStarsText = completeStarsText != null ? completeStarsText : FindPanelText("CompleteStarsText");
        completeScoreText = completeScoreText != null ? completeScoreText : FindPanelText("CompleteScoreText");
        completeHintText = completeHintText != null ? completeHintText : FindPanelText("CompleteHintText");
        // -> Fügt automatisch das StarDisplayUI aus dem Endscreen-Panel hinzu (falls vorhanden)
endscreenStarDisplay = endscreenStarDisplay != null ? endscreenStarDisplay : levelCompletePanel.GetComponentInChildren<StarDisplayUI>();
    }

    private TextMeshProUGUI FindPanelText(string childName)
    {
        Transform child = levelCompletePanel.transform.Find(childName);
        return child != null ? child.GetComponent<TextMeshProUGUI>() : null;
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        GameObject activeObject = GameObject.Find(objectName);

        if (activeObject != null)
        {
            return activeObject;
        }

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject candidate = allObjects[i];

            if (candidate.name == objectName && candidate.scene.IsValid())
            {
                return candidate;
            }
        }

        return null;
    }

    private void AutoAssignLevelUIController()
    {
        if (levelUIController != null)
        {
            return;
        }

        levelUIController = FindObjectOfType<LevelUIController>();

        if (levelUIController == null)
        {
            Debug.LogWarning("RepairCompletionController could not find a LevelUIController in the scene.");
        }
    }
}

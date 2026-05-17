using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HoleSequenceController : MonoBehaviour
{
    public NeedleMover needleMover;
    public TimingEvaluator timingEvaluator;
    public TimingPointerSwing timingPointerSwing;
    public StitchingFeedbackLine stitchingFeedbackLine;
    public GameplayUIController gameplayUIController;
    public RestartController restartController;
    public ScoreController scoreController;
    public StarRatingController starRatingController;
    public Transform timingMinigameRoot;
    public bool hideTimingMinigameOnComplete = true;
    public Vector3 timingOffset = new Vector3(0f, 1.2f, 0f);
    public float timingRotationOffset = 0f;
    public float missRestartDelay = 0.5f;
    public float hitPauseDelay = 0.15f;
    public Transform[] holes;
    public UnityEvent OnLevelCompleted = new UnityEvent();

    private int currentHoleIndex;
    private bool isLevelComplete;
    private Coroutine timingResultRoutine;

    public Transform CurrentHole
    {
        get
        {
            if (holes == null || currentHoleIndex < 0 || currentHoleIndex >= holes.Length)
            {
                return null;
            }

            return holes[currentHoleIndex];
        }
    }

    private void OnEnable()
    {
        SetTimingInputEnabled(false);

        if (timingEvaluator != null)
        {
            timingEvaluator.TimingEvaluated += HandleTimingEvaluated;
        }
    }

    private void OnDisable()
    {
        if (timingResultRoutine != null)
        {
            StopCoroutine(timingResultRoutine);
            timingResultRoutine = null;
        }

        if (timingEvaluator != null)
        {
            timingEvaluator.TimingEvaluated -= HandleTimingEvaluated;
        }
    }

    private void OnValidate()
    {
        missRestartDelay = Mathf.Max(0f, missRestartDelay);
        hitPauseDelay = Mathf.Max(0f, hitPauseDelay);
    }

    private IEnumerator Start()
    {
        yield return null;

        if (needleMover == null)
        {
            Debug.LogWarning("HoleSequenceController needs a NeedleMover reference.");
            yield break;
        }

        if (holes == null || holes.Length == 0 || holes[0] == null)
        {
            Debug.LogWarning("HoleSequenceController needs at least one hole.");
            yield break;
        }

        currentHoleIndex = 0;
        isLevelComplete = false;
        SetTimingMinigameVisible(true);
        needleMover.transform.position = holes[currentHoleIndex].position;
        ResetStitchingFeedback();
        UpdateTimingMinigameTransform();
        ResetTimingSwing();
        ResumeTimingSwing();
        AudioManager.Instance?.PlayTimingStart();
        ResetGameplayUI();
        ResetScore();
        // Set the maximum possible score based on number of holes
        scoreController.SetMaximumScore(holes.Length);
        ResetStars();
        SetTimingInputEnabled(true);

        Debug.Log($"Hole {currentHoleIndex + 1}/{holes.Length}");
    }

    private void HandleTimingEvaluated(TimingResult result)
    {
        if (isLevelComplete || needleMover == null || holes == null || holes.Length == 0)
        {
            return;
        }

        if (timingResultRoutine != null)
        {
            return;
        }

        SetTimingInputEnabled(false);
        PauseTimingSwing();
        ShowTimingResult(result);
        AudioManager.Instance?.PlayHitResult(result);

        if (result == TimingResult.Miss)
        {
            // Reset live stars when the player misses
            if (gameplayUIController != null)
                gameplayUIController.ResetStarDisplay();

            if (starRatingController != null)
                starRatingController.ResetStars();

            timingResultRoutine = StartCoroutine(RestartLevelAfterMiss());
            return;
        }

        AddScoreForTimingResult(result);
        timingResultRoutine = StartCoroutine(HandleTimingResult(result));
    }

    private IEnumerator HandleTimingResult(TimingResult result)
    {
        yield return new WaitForSeconds(hitPauseDelay);

        if (isLevelComplete)
        {
            timingResultRoutine = null;
            yield break;
        }

        int nextHoleIndex = currentHoleIndex + 1;
        if (nextHoleIndex >= holes.Length)
        {
            AddStitchPoint(holes[currentHoleIndex].position);
            CompleteLevel();
            timingResultRoutine = null;
            yield break;
        }

        if (holes[nextHoleIndex] == null)
        {
            Debug.LogWarning($"Hole {nextHoleIndex + 1} is missing.");
            RestartTimingAtCurrentHole();
            timingResultRoutine = null;
            yield break;
        }

        currentHoleIndex = nextHoleIndex;
        UpdateTimingMinigameTransform();
        UpdateGameplayProgress();
        SetStitchTailVisible(true);
        needleMover.MoveTo(holes[currentHoleIndex].position);

        Debug.Log($"Hole {currentHoleIndex + 1}/{holes.Length}");

        while (needleMover.IsMoving)
        {
            if (isLevelComplete)
            {
                timingResultRoutine = null;
                yield break;
            }

            UpdateStitchTail();
            yield return null;
        }

        AddStitchPoint(holes[currentHoleIndex].position);
        UpdateStitchTail();
        RestartTimingAtCurrentHole();
        timingResultRoutine = null;
    }

    private IEnumerator RestartLevelAfterMiss()
    {
        yield return new WaitForSeconds(missRestartDelay);

        if (restartController != null)
        {
            restartController.RestartCurrentScene();
        }
        else
        {
            Debug.LogWarning("HoleSequenceController needs a RestartController reference to restart after a miss.");
        }

        timingResultRoutine = null;
    }

    private void RestartTimingAtCurrentHole()
    {
        if (isLevelComplete)
        {
            return;
        }

        UpdateTimingMinigameTransform();
        ResetTimingSwing();
        ResumeTimingSwing();
        AudioManager.Instance?.PlayTimingStart();
        SetTimingInputEnabled(true);
    }

    private void CompleteLevel()
    {
        if (isLevelComplete)
        {
            return;
        }

        isLevelComplete = true;
        SetTimingInputEnabled(false);
        PauseTimingSwing();

        if (hideTimingMinigameOnComplete)
        {
            SetTimingMinigameVisible(false);
        }

        SetStitchTailVisible(false);
        ShowLevelComplete();
        ShowStarsForFinalScore();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.levelComplete);
        OnLevelCompleted?.Invoke();
        Debug.Log("Level complete");
    }

    private void ResetGameplayUI()
    {
        if (gameplayUIController == null)
        {
            return;
        }

        gameplayUIController.ClearResult();
        gameplayUIController.ShowHint("Click or press Space to stitch");
        UpdateGameplayProgress();
    }

    private void ResetScore()
    {
        if (scoreController != null)
        {
            scoreController.ResetScore();
        }
    }

    private void AddScoreForTimingResult(TimingResult result)
    {
        if (scoreController != null)
        {
            scoreController.AddScoreForResult(result);
            // Update live star display
starRatingController.UpdateStarsFromScore();
if (gameplayUIController != null)
{
    gameplayUIController.UpdateLiveStarDisplay(starRatingController.CurrentStarCount);
}
        }
    }

    private void ResetStars()
    {
        if (starRatingController != null)
        {
            starRatingController.ResetStars();
        }
    }

    private void ShowStarsForFinalScore()
    {
        if (starRatingController == null || scoreController == null)
        {
            return;
        }

        starRatingController.ShowStarsForScore(scoreController.CurrentScore);
    }

    private void ShowTimingResult(TimingResult result)
    {
        if (gameplayUIController != null)
        {
            gameplayUIController.ShowResult(result);
        }
    }

    private void UpdateGameplayProgress()
    {
        if (gameplayUIController != null)
        {
            gameplayUIController.UpdateProgress(currentHoleIndex, holes.Length);
        }
    }

    private void ShowLevelComplete()
    {
        if (gameplayUIController != null)
        {
            gameplayUIController.ShowLevelComplete();
        }
    }

    private void ResetStitchingFeedback()
    {
        if (stitchingFeedbackLine == null)
        {
            return;
        }

        stitchingFeedbackLine.ResetThread();
        stitchingFeedbackLine.SetNeedleTransform(needleMover.transform);
        stitchingFeedbackLine.AddStitchPoint(holes[currentHoleIndex].position);
        stitchingFeedbackLine.SetTailVisible(true);
    }

    private void AddStitchPoint(Vector3 worldPosition)
    {
        if (stitchingFeedbackLine != null)
        {
            stitchingFeedbackLine.AddStitchPoint(worldPosition);
        }
    }

    private void SetStitchTailVisible(bool visible)
    {
        if (stitchingFeedbackLine != null)
        {
            stitchingFeedbackLine.SetTailVisible(visible);
        }
    }

    private void UpdateStitchTail()
    {
        if (stitchingFeedbackLine != null)
        {
            stitchingFeedbackLine.UpdateTailToNeedle();
        }
    }

    private void SetTimingMinigameVisible(bool visible)
    {
        if (timingMinigameRoot != null)
        {
            timingMinigameRoot.gameObject.SetActive(visible);
        }
    }

    private void SetTimingInputEnabled(bool enabled)
    {
        if (timingEvaluator != null)
        {
            timingEvaluator.SetInputEnabled(enabled);
        }
    }

    private void PauseTimingSwing()
    {
        if (timingPointerSwing != null)
        {
            timingPointerSwing.PauseSwing();
        }
        AudioManager.Instance?.StopLoop();
    }

    private void ResumeTimingSwing()
    {
        if (timingPointerSwing != null)
        {
            timingPointerSwing.ResumeSwing();
        }
        AudioManager.Instance?.PlayLoop(AudioManager.Instance.timingSwingLoop);
    }

    private void ResetTimingSwing()
    {
        if (timingPointerSwing != null)
        {
            timingPointerSwing.ResetSwing();
        }
    }

    private void UpdateTimingMinigameTransform()
    {
        if (isLevelComplete || timingMinigameRoot == null || holes[currentHoleIndex] == null)
        {
            return;
        }

        timingMinigameRoot.position = holes[currentHoleIndex].position + timingOffset;

        if (TryGetDirectionToNextHole(out Vector3 directionToNextHole))
        {
            float angle = Mathf.Atan2(directionToNextHole.y, directionToNextHole.x) * Mathf.Rad2Deg;
            timingMinigameRoot.rotation = Quaternion.Euler(0f, 0f, angle - 90f + timingRotationOffset);
        }
    }

    private bool TryGetDirectionToNextHole(out Vector3 directionToNextHole)
    {
        directionToNextHole = Vector3.zero;

        if (holes == null || currentHoleIndex < 0 || currentHoleIndex >= holes.Length || holes[currentHoleIndex] == null)
        {
            return false;
        }

        int nextHoleIndex = currentHoleIndex + 1;

        if (nextHoleIndex >= holes.Length || holes[nextHoleIndex] == null)
        {
            return false;
        }

        directionToNextHole = holes[nextHoleIndex].position - holes[currentHoleIndex].position;
        return directionToNextHole.sqrMagnitude > 0.0001f;
    }
}

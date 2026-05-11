using System.Collections;
using UnityEngine;

public class HoleSequenceController : MonoBehaviour
{
    public NeedleMover needleMover;
    public TimingEvaluator timingEvaluator;
    public Transform timingMinigameRoot;
    public Vector3 timingOffset = new Vector3(0f, 1.2f, 0f);
    public float timingRotationOffset = 0f;
    public Transform[] holes;

    private int currentHoleIndex;
    private bool levelComplete;

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
        if (timingEvaluator != null)
        {
            timingEvaluator.TimingEvaluated += HandleTimingEvaluated;
        }
    }

    private void OnDisable()
    {
        if (timingEvaluator != null)
        {
            timingEvaluator.TimingEvaluated -= HandleTimingEvaluated;
        }
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
        levelComplete = false;
        needleMover.transform.position = holes[currentHoleIndex].position;
        UpdateTimingMinigameTransform();

        Debug.Log($"Hole {currentHoleIndex + 1}/{holes.Length}");
    }

    private void HandleTimingEvaluated(TimingResult result)
    {
        if (levelComplete || needleMover == null || holes == null || holes.Length == 0)
        {
            return;
        }

        if (result == TimingResult.Miss)
        {
            return;
        }

        int nextHoleIndex = currentHoleIndex + 1;

        if (nextHoleIndex >= holes.Length)
        {
            levelComplete = true;
            Debug.Log("Level complete");
            return;
        }

        if (holes[nextHoleIndex] == null)
        {
            Debug.LogWarning($"Hole {nextHoleIndex + 1} is missing.");
            return;
        }

        currentHoleIndex = nextHoleIndex;
        needleMover.MoveTo(holes[currentHoleIndex].position);
        UpdateTimingMinigameTransform();

        Debug.Log($"Hole {currentHoleIndex + 1}/{holes.Length}");
    }

    private void UpdateTimingMinigameTransform()
    {
        if (timingMinigameRoot == null || holes[currentHoleIndex] == null)
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StitchingFeedbackLine : MonoBehaviour
{
    [FormerlySerializedAs("lineRenderer")]
    public LineRenderer completedThreadLine;
    public LineRenderer threadTailLine;

    [Header("Thread Style")]
    public Color threadColor = Color.white;
    [FormerlySerializedAs("lineWidth")]
    public float completedThreadWidth = 0.05f;
    public float tailThreadWidth = 0.05f;
    public string sortingLayerName = "Default";
    public int sortingOrder;
    public float zOffset = 0f;

    private readonly List<Vector3> stitchPoints = new List<Vector3>();
    private Transform needleTransform;
    private bool tailVisible;

    private void Awake()
    {
        InitializeLineRenderers();
    }

    private void Start()
    {
        InitializeLineRenderers();
        ApplyVisualSettings();
    }

    private void OnValidate()
    {
        completedThreadWidth = Mathf.Max(0f, completedThreadWidth);
        tailThreadWidth = Mathf.Max(0f, tailThreadWidth);

        InitializeLineRenderers();
        ApplyVisualSettings();
    }

    public void ResetThread()
    {
        InitializeLineRenderers();

        stitchPoints.Clear();

        if (completedThreadLine != null)
        {
            completedThreadLine.positionCount = 0;
        }

        ClearTail();
    }

    public void AddStitchPoint(Vector3 worldPosition)
    {
        InitializeLineRenderers();

        if (completedThreadLine == null)
        {
            return;
        }

        Vector3 stitchPosition = ApplyZOffset(worldPosition);

        if (stitchPoints.Count > 0 && Vector3.Distance(stitchPoints[stitchPoints.Count - 1], stitchPosition) <= 0.001f)
        {
            return;
        }

        stitchPoints.Add(stitchPosition);
        completedThreadLine.positionCount = stitchPoints.Count;
        completedThreadLine.SetPosition(stitchPoints.Count - 1, stitchPosition);
        UpdateTailToNeedle();
    }

    public void SetThreadPoints(List<Vector3> points)
    {
        ResetThread();

        if (points == null)
        {
            return;
        }

        for (int i = 0; i < points.Count; i++)
        {
            AddStitchPoint(points[i]);
        }
    }

    public void RebuildCompletedThreadFromTransforms(IList<Transform> pointTransforms)
    {
        InitializeLineRenderers();

        stitchPoints.Clear();

        if (completedThreadLine == null)
        {
            return;
        }

        if (pointTransforms == null)
        {
            completedThreadLine.positionCount = 0;
            return;
        }

        for (int i = 0; i < pointTransforms.Count; i++)
        {
            if (pointTransforms[i] != null)
            {
                stitchPoints.Add(ApplyZOffset(pointTransforms[i].position));
            }
        }

        completedThreadLine.positionCount = stitchPoints.Count;

        for (int i = 0; i < stitchPoints.Count; i++)
        {
            completedThreadLine.SetPosition(i, stitchPoints[i]);
        }

        UpdateTailToNeedle();
    }

    public void SetNeedleTransform(Transform needle)
    {
        needleTransform = needle;
        UpdateTailToNeedle();
    }

    public void SetTailVisible(bool visible)
    {
        tailVisible = visible;

        if (!tailVisible)
        {
            ClearTail();
            return;
        }

        UpdateTailToNeedle();
    }

    public void UpdateTailToNeedle()
    {
        InitializeLineRenderers();

        if (!tailVisible || threadTailLine == null || needleTransform == null || stitchPoints.Count == 0)
        {
            ClearTail();
            return;
        }

        threadTailLine.positionCount = 2;
        threadTailLine.SetPosition(0, stitchPoints[stitchPoints.Count - 1]);
        threadTailLine.SetPosition(1, ApplyZOffset(needleTransform.position));
    }

    private void Update()
    {
        if (tailVisible)
        {
            UpdateTailToNeedle();
        }
    }

    private void InitializeLineRenderers()
    {
        if (completedThreadLine == null)
        {
            Transform threadLine = FindChildOrSibling("ThreadLine");
            completedThreadLine = threadLine != null ? threadLine.GetComponent<LineRenderer>() : GetComponent<LineRenderer>();
        }

        if (threadTailLine == null)
        {
            Transform threadTail = FindChildOrSibling("ThreadTail");

            if (threadTail != null)
            {
                threadTailLine = threadTail.GetComponent<LineRenderer>();
            }
        }
    }

    private Transform FindChildOrSibling(string objectName)
    {
        Transform found = transform.Find(objectName);

        if (found != null)
        {
            return found;
        }

        if (transform.parent != null)
        {
            return transform.parent.Find(objectName);
        }

        return null;
    }

    private void ApplyVisualSettings()
    {
        ApplyLineSettings(completedThreadLine, completedThreadWidth);
        ApplyLineSettings(threadTailLine, tailThreadWidth);
    }

    private void ApplyLineSettings(LineRenderer targetLine, float width)
    {
        if (targetLine == null)
        {
            return;
        }

        targetLine.useWorldSpace = true;
        targetLine.startWidth = width;
        targetLine.endWidth = width;
        targetLine.startColor = threadColor;
        targetLine.endColor = threadColor;
        targetLine.sortingLayerName = sortingLayerName;
        targetLine.sortingOrder = sortingOrder;
    }

    private void ClearTail()
    {
        if (threadTailLine != null)
        {
            threadTailLine.positionCount = 0;
        }
    }

    private Vector3 ApplyZOffset(Vector3 worldPosition)
    {
        worldPosition.z += zOffset;
        return worldPosition;
    }
}

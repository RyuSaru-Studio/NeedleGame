using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class TimingArcVisualizer : MonoBehaviour
{
    public float radius = 2f;
    public int segments = 32;
    public float arcWidth = 0.1f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        DrawArc();
    }

    private void OnValidate()
    {
        radius = Mathf.Max(0f, radius);
        segments = Mathf.Max(2, segments);
        arcWidth = Mathf.Max(0f, arcWidth);

        DrawArc();
    }

    private void DrawArc()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.startWidth = arcWidth;
        lineRenderer.endWidth = arcWidth;

        for (int i = 0; i <= segments; i++)
        {
            float progress = i / (float)segments;
            float angle = Mathf.Lerp(180f, 0f, progress) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}

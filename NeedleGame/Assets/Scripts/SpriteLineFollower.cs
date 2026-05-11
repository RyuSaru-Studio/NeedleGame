using UnityEngine;

public class SpriteLineFollower : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float moveSpeed = 1f;
    public float rotationOffset = 0f;
    public bool pingPong = true;

    private float distanceAlongLine;
    private float direction = 1f;
    private float currentAngle;

    public float CurrentAngle
    {
        get { return currentAngle; }
    }

    public float GetCurrentAngle()
    {
        return currentAngle;
    }

    private void Update()
    {
        if (lineRenderer == null || lineRenderer.positionCount < 2 || moveSpeed <= 0f)
        {
            return;
        }

        float lineLength = GetLineLength();

        if (lineLength <= 0f)
        {
            return;
        }

        distanceAlongLine += direction * moveSpeed * Time.deltaTime;

        if (distanceAlongLine >= lineLength)
        {
            distanceAlongLine = lineLength;

            if (pingPong)
            {
                direction = -1f;
            }
            else
            {
                distanceAlongLine = 0f;
            }
        }
        else if (distanceAlongLine <= 0f)
        {
            distanceAlongLine = 0f;
            direction = 1f;
        }

        MoveToDistance(distanceAlongLine);
    }

    private float GetLineLength()
    {
        float length = 0f;

        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 start = GetWorldPoint(i);
            Vector3 end = GetWorldPoint(i + 1);
            length += Vector3.Distance(start, end);
        }

        return length;
    }

    private void MoveToDistance(float targetDistance)
    {
        float traveled = 0f;

        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 start = GetWorldPoint(i);
            Vector3 end = GetWorldPoint(i + 1);
            float segmentLength = Vector3.Distance(start, end);

            if (segmentLength <= 0f)
            {
                continue;
            }

            if (traveled + segmentLength >= targetDistance)
            {
                float segmentProgress = (targetDistance - traveled) / segmentLength;
                Vector3 position = Vector3.Lerp(start, end, segmentProgress);
                Vector3 lineDirection = end - start;

                transform.position = position;
                currentAngle = GetAngleFromLineCenter(position);
                RotateToward(lineDirection);
                return;
            }

            traveled += segmentLength;
        }

        Vector3 finalDirection = GetWorldPoint(lineRenderer.positionCount - 1) - GetWorldPoint(lineRenderer.positionCount - 2);
        transform.position = GetWorldPoint(lineRenderer.positionCount - 1);
        currentAngle = GetAngleFromLineCenter(transform.position);
        RotateToward(finalDirection);
    }

    private Vector3 GetWorldPoint(int index)
    {
        Vector3 point = lineRenderer.GetPosition(index);

        if (lineRenderer.useWorldSpace)
        {
            return point;
        }

        return lineRenderer.transform.TransformPoint(point);
    }

    private float GetAngleFromLineCenter(Vector3 worldPosition)
    {
        Vector3 localPosition = lineRenderer.transform.InverseTransformPoint(worldPosition);
        float angle = Mathf.Atan2(localPosition.y, localPosition.x) * Mathf.Rad2Deg;

        if (angle < 0f)
        {
            angle += 360f;
        }

        return angle;
    }

    private void RotateToward(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }
}

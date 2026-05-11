using UnityEngine;

public class TimingPointerSwing : MonoBehaviour
{
    public float radius = 1f;
    public float swingSpeed = 90f;
    public float minAngle = 0f;
    public float maxAngle = 180f;

    private float currentAngle;
    private float direction = 1f;

    public float CurrentAngle
    {
        get { return currentAngle; }
    }

    public float GetCurrentAngle()
    {
        return currentAngle;
    }

    private void OnValidate()
    {
        radius = Mathf.Max(0f, radius);
        swingSpeed = Mathf.Max(0f, swingSpeed);

        if (maxAngle < minAngle)
        {
            maxAngle = minAngle;
        }
    }

    private void Start()
    {
        currentAngle = minAngle;
        direction = 1f;
        UpdatePointerPosition();
    }

    private void Update()
    {
        if (maxAngle <= minAngle || swingSpeed <= 0f)
        {
            UpdatePointerPosition();
            return;
        }

        currentAngle += direction * swingSpeed * Time.deltaTime;

        if (currentAngle >= maxAngle)
        {
            currentAngle = maxAngle;
            direction = -1f;
        }
        else if (currentAngle <= minAngle)
        {
            currentAngle = minAngle;
            direction = 1f;
        }

        UpdatePointerPosition();
    }

    private void UpdatePointerPosition()
    {
        float angleRadians = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleRadians) * radius;
        float y = Mathf.Sin(angleRadians) * radius;

        transform.localPosition = new Vector3(x, y, transform.localPosition.z);
    }
}

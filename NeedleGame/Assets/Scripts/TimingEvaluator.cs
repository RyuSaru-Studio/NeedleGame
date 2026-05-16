using System.Reflection;
using UnityEngine;

public enum TimingResult
{
    Perfect,
    Good,
    Miss
}

public class TimingEvaluator : MonoBehaviour
{
    public MonoBehaviour timingPointerSwing;
    public float targetAngle = 90f;
    public float perfectTolerance = 8f;
    public float goodTolerance = 20f;

    public event System.Action<TimingResult> TimingEvaluated;

    private bool inputEnabled = true;

    private void OnValidate()
    {
        perfectTolerance = Mathf.Max(0f, perfectTolerance);
        goodTolerance = Mathf.Max(perfectTolerance, goodTolerance);
    }

    private void Update()
    {
        if (!inputEnabled)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            EvaluateTiming();
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    private void EvaluateTiming()
    {
        if (!TryGetCurrentAngle(out float currentAngle))
        {
            Debug.LogWarning("TimingEvaluator needs a pointer script with a public CurrentAngle property or GetCurrentAngle method.");
            return;
        }

        SetInputEnabled(false);

        float distance = Mathf.Abs(currentAngle - targetAngle);
        TimingResult result;

        if (distance <= perfectTolerance)
        {
            result = TimingResult.Perfect;
        }
        else if (distance <= goodTolerance)
        {
            result = TimingResult.Good;
        }
        else
        {
            result = TimingResult.Miss;
        }

        Debug.Log($"{GetResultText(result)} | Angle: {currentAngle:F1} | Distance: {distance:F1}");
        TimingEvaluated?.Invoke(result);
    }

    private string GetResultText(TimingResult result)
    {
        if (result == TimingResult.Perfect)
        {
            return "PERFECT";
        }

        if (result == TimingResult.Good)
        {
            return "GOOD";
        }

        return "MISS";
    }

    private bool TryGetCurrentAngle(out float currentAngle)
    {
        currentAngle = 0f;

        if (timingPointerSwing == null)
        {
            return false;
        }

        PropertyInfo angleProperty = timingPointerSwing.GetType().GetProperty("CurrentAngle");

        if (angleProperty != null && TryConvertToFloat(angleProperty.GetValue(timingPointerSwing), out currentAngle))
        {
            return true;
        }

        MethodInfo angleMethod = timingPointerSwing.GetType().GetMethod("GetCurrentAngle");

        if (angleMethod != null && TryConvertToFloat(angleMethod.Invoke(timingPointerSwing, null), out currentAngle))
        {
            return true;
        }

        return false;
    }

    private bool TryConvertToFloat(object value, out float floatValue)
    {
        if (value is float floatResult)
        {
            floatValue = floatResult;
            return true;
        }

        if (value is int intResult)
        {
            floatValue = intResult;
            return true;
        }

        floatValue = 0f;
        return false;
    }
}
